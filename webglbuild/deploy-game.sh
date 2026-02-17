#!/bin/bash

# ===== CONFIGURACIÓN =====
BUCKET="outland-drifter"
GAME="outland-drifter"
DISTRIBUTION_ID="E38HZONDW7QLYY"

S3_PATH="s3://$BUCKET/games/$GAME"

echo "🚀 Deploying Unity WebGL to $S3_PATH"
echo "----------------------------------"

# ===== VALIDACIONES =====
if [ ! -f "index.html" ]; then
  echo "❌ index.html no encontrado. Ejecutá el script desde la carpeta del build WebGL."
  exit 1
fi

# ===== HTML =====
echo "📄 Subiendo index.html"
aws s3 cp index.html "$S3_PATH/index.html" \
  --content-type "text/html"

# ===== JS.GZ =====
echo "📦 Subiendo *.js.gz"
aws s3 cp Build/ "$S3_PATH/Build/" \
  --recursive \
  --exclude "*" \
  --include "*.js.gz" \
  --content-type "application/javascript" \
  --content-encoding "gzip"

# ===== WASM.GZ =====
echo "🧠 Subiendo *.wasm.gz"
aws s3 cp Build/ "$S3_PATH/Build/" \
  --recursive \
  --exclude "*" \
  --include "*.wasm.gz" \
  --content-type "application/wasm" \
  --content-encoding "gzip"

# ===== DATA.GZ =====
echo "💾 Subiendo *.data.gz"
aws s3 cp Build/ "$S3_PATH/Build/" \
  --recursive \
  --exclude "*" \
  --include "*.data.gz" \
  --content-type "application/octet-stream" \
  --content-encoding "gzip"

# ===== RESTO BUILD =====
echo "📂 Subiendo resto de Build/"
aws s3 cp Build/ "$S3_PATH/Build/" \
  --recursive \
  --exclude "*.gz"

# ===== TEMPLATE DATA =====
echo "🎨 Subiendo TemplateData/"
aws s3 cp TemplateData/ "$S3_PATH/TemplateData/" \
  --recursive

# ===== INVALIDAR CLOUDFRONT =====
echo "🧹 Invalidando cache CloudFront"
aws cloudfront create-invalidation \
  --distribution-id "$DISTRIBUTION_ID" \
  --paths "/games/$GAME/*"

echo "✅ Deploy completado con éxito"
echo "🌐 https://TU_CLOUDFRONT_DOMAIN/games/$GAME/index.html"

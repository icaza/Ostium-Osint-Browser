#!/bin/bash

echo "======================================================"
echo "   MESSIS - OSINT Investigation Tool by ICAZA MEDIA"
echo "======================================================"
echo ""

if ! command -v node &> /dev/null; then
    echo "[ERROR] Node.js is not installed !"
    echo "Download it on https://nodejs.org/"
    exit 1
fi

if [ ! -d "node_modules" ]; then
    echo "[INFO] Installation of outbuildings..."
    npm install
    if [ $? -ne 0 ]; then
        echo "[ERROR] Failure to install outbuildings"
        exit 1
    fi
    echo ""
fi

echo "[INFO] Messis start..."
echo "[INFO] The application will be accessible on http://localhost:{portselect}"
echo "[INFO] Press Ctrl+C to stop the server"
echo ""

npm start

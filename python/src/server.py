#!/usr/bin/env python
# filepath: /workspaces/quickstart/python/src/server.py
"""
Quickstart API:
/ensure-connection - Submits an EnsureConnection job to Deck
/api/create_link_token - Creates a link token for the Deck widget
/api/webhook - Receives webhook events from Deck
"""

import os
import sys
import logging
from flask import Flask, request, jsonify, session, send_from_directory
from dotenv import load_dotenv
from deck import deck

# Configure logging
logging.basicConfig(
    level=logging.INFO,
    format='%(asctime)s - %(name)s - %(levelname)s - %(message)s'
)
logger = logging.getLogger(__name__)

# Load environment variables
load_dotenv()

app = Flask(__name__)

# FOR DEMO PURPOSES ONLY - Use an actual secret key in production
app.secret_key = "bosco"

# Serve frontend
@app.route('/')
def index():
    return send_from_directory('.', 'index.html')

@app.route('/index.js')
def index_js():
    return send_from_directory('.', 'index.js')

# API endpoints
@app.route('/ensure-connection')
def ensure_connection():
    try:
        response = deck.ensure_connection()
        logger.info(f"EnsureConnection job submitted successfully: {response}")
        return "EnsureConnection triggered. Check your webhook logs."
    except Exception as e:
        logger.error(f"Failed to submit EnsureConnection job: {e}")
        return str(e), 500

@app.route('/api/create_link_token', methods=['POST'])
def create_link_token():
    try:
        token = deck.create_widget_token()
        return jsonify(token)
    except Exception as e:
        logger.error(f"Failed to create link token: {e}")
        return jsonify({"error": str(e)}), 500

@app.route('/api/webhook', methods=['POST'])
def webhook():
    data = request.json
    logger.info(f"Webhook received: {data}")
    return "Webhook received", 200

if __name__ == "__main__":
    port = int(os.getenv("PORT", 8080))
    logger.info(f"Server is running on port {port}")
    logger.info(f"Visit: http://127.0.0.1:{port}")
    
    try:
        app.run(host="0.0.0.0", port=port, debug=debug)
    except KeyboardInterrupt:
        logger.info("Server shutdown requested. Exiting...")
        sys.exit(0)

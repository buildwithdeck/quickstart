#!/usr/bin/env python
# filepath: /workspaces/quickstart/python/src/server.py
"""
Quickstart API:
/api/create_link_token - Creates a link token for the Deck widget
/api/webhook - Receives webhook events from Deck
/api/fetch_payment_methods - Use access token to get payment methods
"""

import os
import sys
import logging
from flask import Flask, request, jsonify, session, send_from_directory
from dotenv import load_dotenv
from deck import deck
from webhook import webhook_handler

# Load environment variables from .env file
load_dotenv(dotenv_path=os.path.join(os.path.dirname(os.path.dirname(__file__)), '.env'))

# Configure logging
logging.basicConfig(
    level=logging.INFO,
    format='%(asctime)s - %(name)s - %(levelname)s - %(message)s'
)
logger = logging.getLogger(__name__)

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
    response = webhook_handler.handle_webhook(data)
    return response, 200

@app.route('/api/fetch_payment_methods')
def fetch_payment_methods():
    try:
        access_token = webhook_handler.database.get("access_token")
        if not access_token:
            return "No access token found. Please link an account first.", 400
        
        deck.submit_job("FetchPaymentMethods", {"access_token": access_token})
        return "FetchPaymentMethods triggered. Check your webhook logs for payment methods."
    except Exception as e:
        logger.error(f"Failed to fetch payment methods: {e}")
        return str(e), 500

if __name__ == "__main__":
    port = int(os.getenv("PORT", 8080))
    logger.info(f"Server is running on port {port}")
    logger.info(f"Visit: http://127.0.0.1:{port}")
    
    try:
        app.run(host="0.0.0.0", port=port, debug=False)
    except KeyboardInterrupt:
        logger.info("Server shutdown requested. Exiting...")
        sys.exit(0)

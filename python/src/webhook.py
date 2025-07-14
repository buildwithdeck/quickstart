#!/usr/bin/env python
# filepath: /workspaces/quickstart/python/src/webhook.py
"""
Webhook handler for Deck events
"""

class WebhookHandler:
    def __init__(self):
        self.database = {}
    
    def persist(self, data):
        """Persist data to the in-memory database."""
        self.database.update(data)
        print(f"✓ Updated database: {self.database}")
    
    def handle_webhook(self, request_data):
        """Handle webhook events from Deck."""
        print("Webhook received:", request_data)
        
        output = request_data.get("output", {})
        webhook_code = request_data.get("webhook_code")
        
        if webhook_code == "EnsureConnection":
            access_token = output.get("access_token")
            if access_token:
                self.persist({"access_token": access_token})
        
        elif webhook_code == "FetchPaymentMethods":
            payment_methods = output.get("payment_methods")
            if payment_methods:
                self.persist({"payment_methods": payment_methods})
        
        return "Webhook received"

# Create a global instance
webhook_handler = WebhookHandler()

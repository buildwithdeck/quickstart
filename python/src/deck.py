#!/usr/bin/env python
# filepath: /workspaces/quickstart/python/src/deck.py
import os
import json
import requests
from typing import Dict, Any, Optional
from dotenv import load_dotenv

# Load environment variables
load_dotenv()

class Deck:
    """Python client for the Deck API."""
    
    def __init__(self):
        self._api = "https://sandbox.deck.co/api/v1"
        self.client_id = os.getenv("DECK_CLIENT_ID")
        self.secret = os.getenv("DECK_SECRET")
        
        if not self.client_id or not self.secret:
            print("Warning: DECK_CLIENT_ID or DECK_SECRET environment variables are not set")
    
    def _post(self, endpoint: str, body_json: Optional[Dict[str, Any]] = None) -> Dict[str, Any]:
        """
        Make a POST request to the Deck API.
        
        Args:
            endpoint: API endpoint path
            body_json: Request body as a dictionary
            
        Returns:
            API response as a dictionary
        """
        if body_json is None:
            body_json = {}
            
        headers = {
            "x-deck-client-id": self.client_id,
            "x-deck-secret": self.secret,
            "Content-Type": "application/json"
        }
        
        try:
            response = requests.post(
                f"{self._api}/{endpoint}",
                headers=headers,
                json=body_json
            )
            
            print(f"Deck API response: {response.status_code} {response.reason}")
            
            response.raise_for_status()  # Raise an exception for 4XX/5XX responses
            return response.json()
            
        except requests.exceptions.RequestException as e:
            print(f"Deck API error: {str(e)}")
            raise
    
    def create_widget_token(self) -> Dict[str, Any]:
        """Create a link token for the Deck widget."""
        return self._post("link/token/create")
    
    def ensure_connection(self) -> Dict[str, Any]:
        """Submit an EnsureConnection job to Deck."""
        return self._post("jobs/submit", {
            "job_code": "EnsureConnection",
            "input": {
                "password": "password",
                "username": "username",
                "source_guid": "a47c74e8-ae4a-425e-8583-922d5515654a"  # Youtube
            }
        })

# Create a singleton instance
deck = Deck()

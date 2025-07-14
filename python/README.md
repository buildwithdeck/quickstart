# Python Quickstart

This is a minimal app that implements Deck using a Flask/Python backend with a basic HTML/vanilla JS frontend.

## TL;DR

```bash

# Install dependencies
pip install -r requirements.txt

# Set up environment variables
cp .env.example .env
# Edit .env with your Deck credentials

# Run the server
python src/server.py

# Tunnel to your local server
npm install -g tunnelmole
tmole 8080 # Register https://your-tunnel-url.com/api/webhook in Deck dashboard
```

Then visit http://127.0.0.1:8080 in your browser.

# Webhook

To receive data from Deck, set up a public webhook URL in your Deck dashboard. 

1. Create a tunnel to your local server using [tunnelmole](https://tunnelmole.com).
2. Set the webhook URL in [Dashboard](https://dashboard.deck.co/). e.g., `https://your-tunnel-url.com/api/webhook`

```
npm install -g tunnelmole
tmole 8080
```

Events sent to the webhook will be logged in the console. You can use this to test your webhook setup.

## Setup and Installation

### Prerequisites

- Python 3.7+
- A Deck account with API credentials

### Installation Steps

1. Clone the repository (if you haven't already):
   ```bash
   git clone https://github.com/yourusername/quickstart.git
   cd quickstart/python
   ```

2. Create and activate a virtual environment (recommended):
   ```bash
   python -m venv venv
   source venv/bin/activate  # On Windows: venv\Scripts\activate
   ```

3. Install the dependencies:
   ```bash
   pip install -r requirements.txt
   ```

4. Set up your environment variables:
   ```bash
   cp .env.example .env
   # Edit .env with your text editor and add your Deck credentials
   ```

5. Run the server:
   ```bash
   python src/server.py
   ```

6. Visit http://127.0.0.1:8080 in your browser to see the app in action.

## Overview

This quickstart demonstrates how to:

1. Create a link token for the Deck widget
2. Initialize the Deck Link SDK in a browser
3. Handle events from the Deck Link SDK
4. Handle webhook events and persist access tokens
5. Use access tokens to fetch payment methods

## Key Files

- `src/server.py` - Flask server that handles API requests
- `src/deck.py` - Python client for the Deck API
- `src/webhook.py` - Webhook handler for processing Deck events
- `src/index.html` - HTML frontend
- `src/index.js` - JavaScript for the frontend

## API Endpoints

- `/` - Serves the frontend
- `/api/create_link_token` - Creates a link token for the Deck widget
- `/api/webhook` - Receives webhook events from Deck
- `/api/fetch_payment_methods` - Use access token to get payment methods

## Example Usage

### Initialize the Deck client

```python
from deck import Deck

# Create a Deck client instance
deck_client = Deck()
```

### Create a widget token

```python
# Create a link token for the Deck widget
token = await deck_client.create_widget_token()
```

### Submit a job to Deck

```python
# Submit any job to Deck
response = await deck_client.submit_job("FetchPaymentMethods", {"access_token": "your_access_token"})
```

### Handling webhook events

```python
from webhook import webhook_handler

@app.route('/api/webhook', methods=['POST'])
def webhook():
    data = request.json
    response = webhook_handler.handle_webhook(data)
    return response, 200
```

### Accessing stored data

```python
# Access stored data from webhooks
access_token = webhook_handler.database.get("access_token")
payment_methods = webhook_handler.database.get("payment_methods")
```

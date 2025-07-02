# Python Quickstart

This is a minimal app that implements Deck using a Flask/Python backend with a basic HTML/vanilla JS frontend.

## TL;DR

```bash
# Tunnel to your local server
npm install -g tunnelmole
tmole 8080 # Register https://your-tunnel-url.com/api/webhook in Deck dashboard

# Install dependencies
pip install -r requirements.txt

# Set up environment variables
cp .env.example .env
# Edit .env with your Deck credentials

# Run the server
python src/server.py
```

Then visit http://127.0.0.1:8080 in your browser.

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
4. Submit an EnsureConnection job
5. Handle webhook events

## Key Files

- `src/server.py` - Flask server that handles API requests
- `src/deck.py` - Python client for the Deck API
- `src/index.html` - HTML frontend
- `src/index.js` - JavaScript for the frontend

## API Endpoints

- `/` - Serves the frontend
- `/ensure-connection` - Submits an EnsureConnection job to Deck
- `/api/create_link_token` - Creates a link token for the Deck widget
- `/api/webhook` - Receives webhook events from Deck

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

### Ensure a connection

```python
# Submit an EnsureConnection job
response = await deck_client.ensure_connection()
```

### Handling webhook events

```python
@app.route('/api/webhook', methods=['POST'])
def webhook():
    # Process the webhook event
    event_data = request.json
    print("Webhook received:", event_data)
    return "Webhook received", 200
```

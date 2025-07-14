# [Deck](https://deck.co) Quickstart (NodeJS)

This is a minimal app that implements Deck using **HTML/vanilla JS frontend** with an **Express/Node backend**.

It demonstrates how to set up a webhook, obtain an access token using the Deck Widget, and fetch user-permissioned data from the Deck API.

# TL;DR

```sh
# Fill out the .env file with your Deck credentials
cp .env.example .env

# Run the local server
pnpm install
pnpm start # http://127.0.0.1:8080

# Tunnel to your local server
npm install -g tunnelmole
tmole 8080 # Register https://your-tunnel-url.com/api/webhook in Deck dashboard
```

# Webhook

To receive data from Deck, set up a public webhook URL in your Deck dashboard. 

1. Create a tunnel to your local server using [tunnelmole](https://tunnelmole.com).
2. Set the webhook URL in [Dashboard](https://dashboard.deck.co/). e.g., `https://your-tunnel-url.com/api/webhook`

```
npm install -g tunnelmole
tmole 8080
```

Events sent to the webhook will be logged in the console. You can use this to test your webhook setup.

# Running the app

It is recommended to use latest stable version of Node. At the time of writing, the latest stable version is v22.16.0. 
For information on installing Node, see [How to install Node.js](https://nodejs.dev/learn/how-to-install-nodejs).

Fill out the contents of the **.env** file with the [client ID and Sandbox secret in your Deck dashboard](https://dashboard.deck.co/). Use the **Sandbox** secret when setting the `DECK_SECRET` variable.

```bash
cp .env.example .env
pnpm run start #  http://127.0.0.1:8080
```

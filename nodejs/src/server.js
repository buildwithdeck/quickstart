/*
 * Quickstart API:
 * /api/create_link_token - Creates a link token for the Deck widget
 * /api/webhook - Receives webhook events from Deck
 * /example/ensure-connection - Get an access token
 * /example/fetch-payment-methods - Use access token to get payment methods
 */

require("dotenv").config();
const express = require("express");
const session = require("express-session");
const { deck } = require("./deck");
const { webhook } = require("./webhook");
const app = express();

app.use(
  // FOR DEMO PURPOSES ONLY
  // Use an actual secret key in production
  session({ secret: "bosco", saveUninitialized: true, resave: true })
);

// Replace body-parser with native express methods
app.use(express.urlencoded({ extended: false }));
app.use(express.json());

// Serve frontend
const serveFile = (path) => (req, res) =>
  res.sendFile(path, { root: __dirname });
app.get("/", serveFile("index.html"));
app.get("/index.js", serveFile("index.js"));

// Serve API
app.post("/api/create_link_token", async (req, res) =>
  res.json(await deck.createWidgetToken())
);

app.post("/api/webhook", webhook.handler);

app.get("/api/fetch_payment_methods", async (req, res) => {
  const { access_token } = webhook.database;
  await deck.submitJob("FetchPaymentMethods", { access_token });
  res.send("FetchPaymentMethods triggered. Check your webhook logs for payment methods.");
});

const PORT = process.env.PORT || 8080;
app.listen(PORT, () => {
  console.log(`Server is running on port ${PORT}`);
  console.log(`http://127.0.0.1:${PORT}`);
});

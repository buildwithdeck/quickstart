/*
 * Quickstart API:
 * /api/create_link_token - Creates a link token for the Deck widget
 * /api/webhook - Receives webhook events from Deck
 */

require("dotenv").config();
const express = require("express");
const session = require("express-session");
const { deck } = require("./deck");
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

app.post("/api/create_link_token", async (req, res) =>
  res.json(await deck.createWidgetToken())
);
app.post("/api/webhook", (req, res) => {
  console.log("Webhook received:", req.body);
  res.status(200).send("Webhook received");
});

const PORT = process.env.PORT || 8080;
app.listen(PORT, () => {
  console.log(`Server is running on port ${PORT}`);
  console.log(`http://127.0.0.1:${PORT}`);
});

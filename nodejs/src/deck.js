export const deck = {
  _api: "https://sandbox.deck.co/api/v1",
  async _post(endpoint, bodyJson = {}) {
    const response = await fetch(`${this._api}/${endpoint}`, {
      method: "POST",
      headers: {
        "x-deck-client-id": process.env.DECK_CLIENT_ID,
        "x-deck-secret": process.env.DECK_SECRET,
        "Content-Type": "application/json",
      },
      body: JSON.stringify(bodyJson),
    });
    console.log("Deck API response:", response.status, response.statusText);
    if (!response.ok) {
      throw new Error(`Deck API error: ${response.status} ${response.statusText}`);
    }
    return await response.json();
  },
  createWidgetToken() {
    return this._post("link/token/create");
  },
  ensureConnection() {
    return this._post("jobs/submit", {
      job_code: "EnsureConnection",
      input: {
        password: "password",
        username: "username",
        source_guid: "a47c74e8-ae4a-425e-8583-922d5515654a" // Youtube
      }
    });
  }
};
(function linkSdkIIFE(context) {
  const server = {
    _api: "http://127.0.0.1:8080",
    _headers: {
      "Content-Type": "application/json",
    },

    async createToken() {
      const response = await fetch(`${this._api}/api/create_link_token`, {
        method: "POST",
        headers: this._headers,
      });

      return response.json();
    },
  };

  function logEvent(eventText) {
    const timestamp = new Date().toLocaleTimeString();
    let eventsElement = document.querySelector(".js-events");
    eventsElement.textContent += `[${timestamp}] ${eventText}\n`;

    // Auto-scroll to the bottom with smooth behavior
    eventsElement.scrollTo({
      top: eventsElement.scrollHeight,
      behavior: "smooth",
    });
  }

  async function startLink(event) {
    const button = event.target || {};
    button.disabled = true;

    const { link_token: token } = await server.createToken();

    const handler = Deck.create({
      token,
      // A single source can be specified, this will skip the source select screen.
      // For the skip to work, make sure that the source specified here would appear normally on the source select screen.
      // source_id: "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      onExit() {
        logEvent("onExit()");
        button.disabled = false;
      },
      onError() {
        logEvent("onError()");
        button.disabled = false;
      },
      async onSuccess({ public_token }) {
        logEvent("onSuccess()");
        button.disabled = false;
      },
    });

    handler.open();
  }

  context.startLink = startLink;
})(window);

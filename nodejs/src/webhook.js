const persist = (data) => {
  webhook.database = { ...webhook.database, ...data }
  console.log("✓ Updated database: ", webhook.database); 
}

export const webhook = {
  database: {},
  handler: (req, res) => {
    console.log("Webhook received:", req.body);
    const { output, webhook_code } = req.body;

    switch (webhook_code) {
      case "EnsureConnection":
        const { access_token } = output;
        persist({ access_token });
        break;
      case "FetchPaymentMethods":
        const { payment_methods } = output;
        persist({ payment_methods });
        break;
    }
    res.status(200).send("Webhook received");
  }
};

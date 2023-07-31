// The chatId here should be specific to the whitelabel instance.
// This file can/should be put in place via the Docker compose processs
// '44d8ada3cdcb4c2c209f4457d3a4ebc0' = countryhouseescapes.com

setTimeout(showChat, 10000);

function showChat() {
    window.FrontChat('init', { chatId: '44d8ada3cdcb4c2c209f4457d3a4ebc0' });
}

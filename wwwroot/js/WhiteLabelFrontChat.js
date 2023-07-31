// The chatId here should be specific to the whitelabel instance.
// This file can/should be put in place via the Docker compose processs
// 'f28bf48aaa0144b8c4e4d1befc803cd3' = leavetown.com

setTimeout(showChat, 10000);

function showChat() {
    window.FrontChat('init', { chatId: 'f28bf48aaa0144b8c4e4d1befc803cd3' });
}
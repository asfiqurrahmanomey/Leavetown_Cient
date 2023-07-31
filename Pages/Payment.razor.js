function loadStripePaymentFields(privateKey, clientSecret, currencyCode, genericError) {
    const stripe = Stripe(privateKey);
    const style = getComputedStyle(document.body);
    const appearance = {
        theme: 'stripe',
        variables: {
            fontFamily: style.getPropertyValue('--fonts-body'),
            colorText: style.getPropertyValue('--colors-text'),
            colorDanger: style.getPropertyValue('--colors-red-500'),
            borderRadius: style.getPropertyValue('--rz-input-border-radius')
        }
    };
    const options = {};
    const elements = stripe.elements({ clientSecret, appearance });
    const paymentElement = elements.create('payment', options);

    paymentElement.mount('#stripe-container');

    const form = document.getElementById('payment-form');
    let submitted = false;

    form.addEventListener('submit', async (e) => {
        e.preventDefault();

        // Disable double submission of the form
        if (submitted) {
            return;
        }

        submitted = true;
        form.querySelector('button').disabled = true;

        // Confirm the payment given the clientKey from the payment intent that was just created on the server.
        await stripe.confirmPayment({
            elements,
            confirmParams: {
                return_url: `${window.location.origin}/payment/thankyou?c=${currencyCode}`,
            }
        }).then(function (result) {
            if (result.error && result.error.type !== 'validation_error') {
                form.querySelector('#error-message').textContent = genericError;
            }

            form.querySelector('button').disabled = false;
            submitted = false;
        });
    });
}

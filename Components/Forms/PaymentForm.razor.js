var GLOBAL = {};
GLOBAL.DotNetReference = null;
GLOBAL.SetDotnetReference = function (pDotNetReference) {
    GLOBAL.DotNetReference = pDotNetReference;
};

var stripe;
var isCardCompleteAndValid = false;
var isTermsAndConditionsChecked = false;

function setStripeAccount(apiKey) {
    stripe = Stripe(apiKey);
}

async function initialize() {
    // Disable the button until we have Stripe set up on the page
    document.querySelector("#submit").disabled = true;

    var elements = stripe.elements();
    var style = {
        base: {
            color: "#32325d",
            fontFamily: 'Arial, sans-serif',
            fontSmoothing: "antialiased",
            fontSize: "16px",
            "::placeholder": {
                color: "#32325d"
            }
        },
        invalid: {
            fontFamily: 'Arial, sans-serif',
            color: "#fa755a",
            iconColor: "#fa755a"
        }
    };

    var card = elements.create("card", { style: style });
    // Stripe injects an iframe into the DOM
    card.mount("#card-element");   

    card.on("change", function (event) {
        isCardCompleteAndValid = event.complete && !event.error;    
        showOrHideStripeError(event.error);
        setConfirmAndPayButtonStatus();
    });

    var form = document.getElementById("payment-form");
    form.addEventListener("submit", function (event) {
        event.preventDefault();
        document.querySelector("#submit").disabled = true; // Disable button to prevent duplicate bookings.

        // Complete payment when the submit button is clicked
        stripe.createToken(card).then(function (result) {            
            GLOBAL.DotNetReference.invokeMethodAsync('SubmitToken', result);                        
        });
    });
}

function showOrHideStripeError(error) {
    document.querySelector("#card-error").textContent = error ? error.message : "";
    document.querySelector("#card-error-container").className = error ? "error" : " error hidden";
}

function onTermsAndConditionsChange(termsAndConditionsCheckboxIsChecked) {
    isTermsAndConditionsChecked = termsAndConditionsCheckboxIsChecked;
    setConfirmAndPayButtonStatus();
}

function setConfirmAndPayButtonStatus() {
    document.querySelector("#submit").disabled = !isCardCompleteAndValid || !isTermsAndConditionsChecked;
}
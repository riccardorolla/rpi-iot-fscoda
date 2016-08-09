var MsTranslator = require('mstranslator');
// Second parameter to constructor (true) indicates that
// the token should be auto-generated.
var client = new MsTranslator({
  client_id: "skqV8Yw06YdPuyBhthQqB5TDyEbmMNqf6OmKWNGwjnA"
  , client_secret: "skqV8Yw06YdPuyBhthQqB5TDyEbmMNqf6OmKWNGwjnA"
}, true);

var params = {
  text: 'How\'s it going?'
  , from: 'en'
  , to: 'es'
};

// Don't worry about access token, it will be auto-generated if needed.
client.translate(params, function(err, data) {
  console.log(data);
});
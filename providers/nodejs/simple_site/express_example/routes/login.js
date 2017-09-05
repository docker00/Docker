var express = require('express');
var router = express.Router();

var oidcConfig = {
 scope: 'profile roles offline_access', // offline is not required for code flow, but is typically used in this flow to get refresh tokens 
 client_id: 'implicit_client',
 callbackURL: '/auth/oidc/callback',
 authority: 'https://localhost:5000/',
 response_type: "code", 
 response_mode: "form_post",
 scopeSeparator: ' ',
 verbose_logging: true
};

router.get('/auth/oidc/login',
  function (req, res) {
    
    var oidcClient = new OidcClient(req, res, oidcConfig);
        
    var tokenRequest = oidcClient.createTokenRequestAsync();
    
    tokenRequest.then(function (results) {
      console.log('about to redirect');
      res.redirect(results.url);  
    }).catch(function(error){
        console.log('error generating redirect url: ' + error);
    });
});
 
router.post('/auth/oidc/callback',
  function (req, res) {
    
    var oidcClient = new OidcClient(req, res, oidcConfig);
    
    var tokenResponse = oidcClient.processResponseAsync(req.body);
    
    tokenResponse.then(function (results) {
      console.log(results);
    }).catch(function(error) {
        console.log('error parsing token response: ' + error);
    });
    
    console.log('Made it to the end of the response function');
});

router.get('/token/refresh/', function(req, res) {
  var oidcClient = new OidcClient(req, res, oidcConfig);
  oidcClient.mergeRequestOptions(req, {});
  
  var refreshToken = getRefreshToken(); // get the current refresh token you have persisted somewhere 
  
  oidcClient.refreshAccessTokenAsync(refreshToken).then(function (tokenResponse) {
    handleTokenResponse(tokenResponse); // do something with the token you received 
    res.redirect('/');
  });
});

module.exports = router;

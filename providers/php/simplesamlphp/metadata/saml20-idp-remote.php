<?php
/**
 * SAML 2.0 remote IdP metadata for SimpleSAMLphp.
 *
 * Remember to remove the IdPs you don't use from this file.
 *
 * See: https://simplesamlphp.org/docs/stable/simplesamlphp-reference-idp-remote 
 */

/*
 * Guest IdP. allows users to sign up and register. Great for testing!
 */

$metadata['http://localhost:5000'] = array(
	'name' => array(
		'en' => 'IdentityServer4',
	),
	'description'          => 'Here you can login with your account on IdentityServer4 OpenID. If you do not already have an account on this identity provider, you can create a new one by following the create new account link and follow the instructions.',

	'auth' => 'openid-connect',

'authproc' => array(
        60 => array(
                'class' => 'authorize:Authorize',
        ),
      ),


	//'SingleSignOnService'  => 'http://localhost/simplesaml/saml2/idp/SSOService.php',
//	'SingleSignOnService'  => 'http://localhost:5000/connect/authorize',
	//'SingleLogoutService'  => 'http://localhost/simplesaml/saml2/idp/SingleLogoutService.php',
//	'SingleLogoutService'  => 'http://localhost:5000/connect/endsession',
);


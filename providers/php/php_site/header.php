<?php

/**
 *
 * Copyright MITRE 2012
 *
 * OpenIDConnectClient for PHP5
 * Author: Michael Jett <mjett@mitre.org>
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may
 * not use this file except in compliance with the License. You may obtain
 * a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations
 * under the License.
 *
 */

require_once("/var/simplesamlphp/lib/_autoload.php");

$as = new SimpleSAML_Auth_Simple('default-sp');

$as->requireAuth();

$attributes = $as->getAttributes();

print_r($attributes);

print_r($as->getAuthDataArray());

print_r($_SERVER);

print_r($_SERVER['HTTP_X_DATA_REQUEST_URI']);
print_r($_SERVER['SERVER_ADDR']);
print_r($_SERVER['SERVER_PORT']); //wtf? 80, not 8080
print_r($_SERVER['SERVER_NAME']);
print_r($_SERVER['REMOTE_ADDR']); //client
print_r($_SERVER['REQUEST_SCHEME']);
print_r($_SERVER['HTTP_REFERER']);

?>

<html>
<head>
    <title>Example OpenID Connect Client Use</title>
    <style>
        body {
            font-family: 'Lucida Grande', Verdana, Arial, sans-serif;
        }
    </style>
</head>
<body>

    <div>
        Hello, <?php echo 'test'; ?>
    </div>

    <div>
        <a href="/php/php_provider/client_example.php">page1</a>
        <a href="/php/php_provider/page2.php">page2</a>
        <a href="/php/php_provider/page3.php">page3</a>
        <a href="/php/php_provider/Admin/config.php">Config page</a>
    </div>

</body>
</html>


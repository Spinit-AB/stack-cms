'use strict';

import angular from 'angular'
import 'angular-ui-bootstrap';
import 'angular-animate';

angular.module('Spinit.Stack.CMS',
[
    'ui.bootstrap',
    'ngAnimate'
]);

var req = require.context("./", true, /^(.*\.(js$))[^.]*$/igm);

req.keys().forEach(function (key) {
    req(key);
});



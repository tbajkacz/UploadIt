/// <reference path="../lib/jquery.min.js" />
import * as constants from "./ApiUrl.js";

(function() {
    $.ajax({
        url: constants.apiUrl  + "/"
    });
});
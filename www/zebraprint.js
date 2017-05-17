var zebraprint = {
    print: function (successCallback, errorCallback, mac, data) {
        cordova.exec(successCallback, errorCallback, "zebraprint", "print", [mac, data]);
    }
}

module.exports = zebraprint;

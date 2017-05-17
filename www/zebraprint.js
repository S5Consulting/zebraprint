var zebraprint = {
    print: function (successCallback, errorCallback, strInput) {
        cordova.exec(successCallback, errorCallback, "zebraprint", "print", [strInput]);
    }
}

module.exports = zebraprint;

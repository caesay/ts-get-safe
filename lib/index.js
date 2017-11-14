var lodashGet = require("lodash.get");

function safeGet() {
    var args = Array.prototype.slice.call(arguments);
    var obj = args[0];
    var path = args.slice(1).join(".");
    return lodashGet(obj, path);
}

module.exports.default = safeGet;
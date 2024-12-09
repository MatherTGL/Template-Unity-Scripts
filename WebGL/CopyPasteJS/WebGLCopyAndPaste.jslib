mergeInto(LibraryManager.library, {
    CopyToClipboard: function (textPtr) {
        var text = UTF8ToString(textPtr);
        navigator.clipboard.writeText(text).then(
            function () {
                console.log("Text copied to clipboard: " + text);
            },
            function (err) {
                console.error("Could not copy text: ", err);
            }
        );
    }
});
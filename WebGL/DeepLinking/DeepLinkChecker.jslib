mergeInto(LibraryManager.library, {
    GetDeepLink: function () {
        var returnStr = window.Telegram.WebApp.initDataUnsafe.start_param || "";
        var bufferSize = lengthBytesUTF8(returnStr) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(returnStr, buffer, bufferSize);
        return buffer;
    }
});
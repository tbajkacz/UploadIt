/// <reference path="../lib/jquery.min.js" />

$(document).ready(() => {

    $("#file_drop_zone").on("drop", (ev) => {
        ev.dataTransfer = ev.originalEvent.dataTransfer;
        ev.preventDefault();
        if (ev.dataTransfer.items) {
            for (var i = 0; i < ev.dataTransfer.items.length; i++) {
                // If dropped items aren't files, reject them
                if (ev.dataTransfer.items[i].kind === 'file') {
                    var file = ev.dataTransfer.items[i].getAsFile();
                    console.log(`file[${i}].name = ${file.name}`);
                }
            }
        }
    });
    /**
     * try with vanilla js -- ev.originalEvent may not be needed
     * check why something like let ar = [...ev.dataTransfer.items] and for...in loop won't work
     */

    $("#file_drop_zone").on("dragover", (ev) => {
        ev.preventDefault();
    });


});
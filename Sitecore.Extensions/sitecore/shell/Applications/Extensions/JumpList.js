scContentEditor.prototype.onJumpListContextMenu = function (tag, evt) {
    if (evt.ctrlKey) {
        return;
    }

    var control = scForm.browser.getSrcElement(evt);

    while (control != null && control.tagName != "A") {
        control = control.parentNode;
    }

    if (control != null) {
        scForm.lastEvent = evt;
        scForm.invoke('Tree_ContextMenu("' + control.id + '")');
        scForm.lastEvent = null;
    }
    else if (evt.clientX < 20) {
        scForm.postRequest("", "", "", "Gutter_ContextMenu");
    }

    scForm.browser.clearEvent(evt, true, false);
}

scContentEditor.prototype.onJumpListClick = function (sender, evt) {
    var ctl = scForm.browser.getSrcElement(evt);

    if (ctl != null) {
        if (ctl.id != null && ctl.id.indexOf("_Glyph_") >= 0) {
            return this.onTreeGlyphClick(ctl, ctl.id.substr(ctl.id.lastIndexOf("_") + 1), this.getID(ctl.id));
        }

        while (ctl != null && ctl.tagName != "A") {
            ctl = ctl.parentNode;
        }

        if (ctl != null) {
            if ($(ctl).hasClassName("scBeenDragged")) {
                console.info("been dragged click");
                Event.stop(evt);
                $(ctl).removeClassName("scBeenDragged");
                return;
            }

            if ($(ctl).hasClassName("scContentTreeNodeStatic")) {
                return;
            }

            if (ctl.id != null && ctl.id.indexOf("_Item_") >= 0) {
                if (!$$('#EditorFrames>iframe[id^="F{"]').any(function (iframe) { return iframe.contentWindow.scForm && iframe.contentWindow.scForm.modified; })
                    || confirm(scForm.translate("There are unsaved changes. Are you sure you want to continue?"))) {
                    return this.onTreeNodeClick(ctl, ctl.id.substr(ctl.id.lastIndexOf("_") + 1));
                }
            }
        }
    }

    this.onEditorClick(sender, evt);

    scForm.browser.clearEvent(evt, true, false);

    return false;
}
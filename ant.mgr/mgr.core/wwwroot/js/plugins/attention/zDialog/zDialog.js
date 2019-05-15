var mainprePath = window.appUrl + '/js/';
var themeColor = "blue";
if ($("#skin").attr("prePath") != null) {
    mainprePath = $("#skin").attr("prePath")
}
var IMAGESPATH = mainprePath + "plugins/attention/zDialog/skins/blue/";
if ($("#skin").attr("themeColor") != null) {
    themeColor = $("#skin").attr("themeColor");
    IMAGESPATH = mainprePath + "plugins/attention/zDialog/skins/" + themeColor + "/"
}
var HideScrollbar = true;
var agt = window.navigator.userAgent;
var isIE = agt.toLowerCase().indexOf("msie") != -1;
var isGecko = agt.toLowerCase().indexOf("gecko") != -1;
var ieVer = isIE ? parseInt(agt.split(";")[1].replace(/(^\s*)|(\s*$)/g, "").split(" ")[1]) : 0;
var isIE8 = !! window.XDomainRequest && !! document.documentMode;
var isIE7 = ieVer == 7 && !isIE8;
var ielt7 = isIE && ieVer < 7;
var isQuirks = document.compatMode == "BackCompat";
var $id = function (a) {
    return typeof a == "string" ? document.getElementById(a) : a
};

function stopEvent(a) {
    a = window.event || a;
    if (!a) {
        return
    }
    if (isGecko) {
        a.preventDefault();
        a.stopPropagation()
    }
    a.cancelBubble = true;
    a.returnValue = false
}
Array.prototype.remove = function (c, b) {
    if (b) {
        var d = [];
        for (var a = 0; a < this.length; a++) {
            if (c == this[a]) {
                d.push(this.splice(a, 1)[0])
            }
        }
        return d
    }
    for (var a = 0; a < this.length; a++) {
        if (c == this[a]) {
            this.splice(a, 1)
        }
    }
    return this
};
if (!isIE && HTMLElement) {
    if (!HTMLElement.prototype.attachEvent) {
        window.attachEvent = document.attachEvent = HTMLElement.prototype.attachEvent = function (b, a) {
            b = b.substring(2);
            this.addEventListener(b, a, false)
        };
        window.detachEvent = document.detachEvent = HTMLElement.prototype.detachEvent = function (b, a) {
            b = b.substring(2);
            this.removeEventListener(b, a, false)
        }
    }
} else {
    if (isIE && ieVer < 8) {
        try {
            document.execCommand("BackgroundImageCache", false, true)
        } catch (e) {}
    }
}
var $topWindow = function () {
    var a = window;
    while (a != a.parent) {
        if (a.parent.document.getElementsByTagName("FRAMESET").length > 0) {
            break
        }
        a = a.parent
    }
    return a
};
var $bodyDimensions = function (j) {
    j = j || window;
    var h = j.document;
    var c = h.compatMode == "BackCompat" ? h.body.clientWidth : h.documentElement.clientWidth;
    var g = h.compatMode == "BackCompat" ? h.body.clientHeight : h.documentElement.clientHeight;
    var b = Math.max(h.documentElement.scrollLeft, h.body.scrollLeft);
    var d = Math.max(h.documentElement.scrollTop, h.body.scrollTop);
    var a = Math.max(h.documentElement.scrollWidth, h.body.scrollWidth);
    var f = Math.max(h.documentElement.scrollHeight, h.body.scrollHeight);
    if (f < g) {
        f = g
    }
    return {
        clientWidth: c,
        clientHeight: g,
        scrollLeft: b,
        scrollTop: d,
        scrollWidth: a,
        scrollHeight: f
    }
};
var fadeEffect = function (b, f, a, c, d) {
    if (!b.effect) {
        b.effect = {
            fade: 0,
            move: 0,
            size: 0
        }
    }
    clearInterval(b.effect.fade);
    var c = c || 20;
    b.effect.fade = setInterval(function () {
        f = f < a ? Math.min(f + c, a) : Math.max(f - c, a);
        b.style.opacity = f / 100;
        b.style.filter = "alpha(opacity=" + f + ")";
        if (f == a) {
            clearInterval(b.effect.fade);
            if (d) {
                d.call(b)
            }
        }
    }, 20)
};
var topWin = $topWindow();
var topDoc = topWin.document;
var Dialog = function () {
    this.ID = null;
    this.Width = 550;
    this.Height = 380;
    this.URL = null;
    this.OnLoad = null;
    this.InnerHtml = "";
    this.InvokeElementId = "";
    this.Top = "50%";
    this.Left = "50%";
    this.Title = "　";
    this.OkButtonText = " 确 定 ";
    this.CancelButtonText = " 取 消 ";
    this.OKEvent = null;
    this.CancelEvent = null;
    this.ShowButtonRow = false;
    this.ShowOkButton = true;
    this.ShowCancelButton = true;
    this.MessageIcon = "window.gif";
    this.MessageTitle = "";
    this.Message = "";
    this.TableBackGroundColor = "";
    this.ShowMessageRow = false;
    this.Modal = true;
    this.Drag = true;
    this.AutoClose = null;
    this.ShowCloseButton = true;
    this.Animator = !ielt7;
    this.MsgForESC = "";
    this.InnerFrameName = null;
    this.dialogDiv = null;
    this.bgDiv = null;
    this.openerWindow = null;
    this.openerDialog = null;
    this.innerFrame = null;
    this.innerWin = null;
    this.innerDoc = null;
    this.zindex = 900;
    this.cancelButton = null;
    this.okButton = null;
    this.unauthorized = false;
    if (arguments.length > 0 && typeof(arguments[0]) == "string") {
        this.ID = arguments[0]
    } else {
        if (arguments.length > 0 && typeof(arguments[0]) == "object") {
            Dialog.setOptions(this, arguments[0])
        }
    }
    if (!this.ID) {
        this.ID = topWin.Dialog._dialogArray.length + ""
    }
};
Dialog._dialogArray = [];
Dialog._childDialogArray = [];
Dialog.bgDiv = null;
Dialog.setOptions = function (c, b) {
    if (!b) {
        return
    }
    for (var a in b) {
        c[a] = b[a]
    }
};
Dialog.attachBehaviors = function () {
    document.attachEvent("onkeydown", Dialog.onKeyDown);
    window.attachEvent("onresize", Dialog.resetPosition);
    if (!HideScrollbar && ielt7) {
        window.attachEvent("onscroll", Dialog.resetPosition)
    }
};
Dialog.prototype.attachBehaviors = function () {
    var c = this;
    if (this.Drag && topWin.Drag) {
        var a = topWin.$id("_Draghandle_" + this.ID),
            b = topWin.$id("_DialogDiv_" + this.ID);
        topWin.Drag.init(a, b);
        b.onDragStart = function (h, g, f, d) {
            if (!isIE && c.URL) {
                topWin.$id("_Covering_" + c.ID).style.display = ""
            }
        };
        b.onDragEnd = function (j, h, f, d) {
            if (!isIE && c.URL) {
                topWin.$id("_Covering_" + c.ID).style.display = "none"
            }
            var g = $bodyDimensions(topWin);
            if (j < 0) {
                this.style.left = "0px"
            }
            if (j + this.clientWidth > g.clientWidth) {
                this.style.left = g.clientWidth - this.clientWidth + "px"
            }
            if (c.fixedPosition) {
                if (h < 0) {
                    this.style.top = "0px"
                }
                if (h + 33 > g.clientHeight) {
                    this.style.top = g.clientHeight - 33 + "px"
                }
            } else {
                if (h < g.scrollTop) {
                    this.style.top = g.scrollTop + "px"
                }
                if (h + 33 > g.scrollTop + g.clientHeight) {
                    this.style.top = g.scrollTop + g.clientHeight - 33 + "px"
                }
            }
        }
    }
};
Dialog.prototype.displacePath = function () {
    if (this.URL.substr(0, 7) == "http://" || this.URL.substr(0, 1) == "/" || this.URL.substr(0, 11) == "javascript:") {
        return this.URL
    } else {
        var a = this.URL;
        var b = window.location.href;
        b = b.substring(0, b.lastIndexOf("/"));
        while (a.indexOf("../") >= 0) {
            a = a.substring(3);
            b = b.substring(0, b.lastIndexOf("/"))
        }
        return b + "/" + a
    }
};
Dialog.prototype.setPosition = function () {
    var d = $bodyDimensions(topWin);
    var g = this.Top,
        b = this.Left,
        c = this.getDialogDiv();
    if (typeof this.Top == "string" && this.Top.indexOf("%") != -1) {
        var f = parseFloat(this.Top) * 0.01;
        g = this.fixedPosition ? d.clientHeight * f - c.scrollHeight * f : d.clientHeight * f - c.scrollHeight * f + d.scrollTop
    }
    if (typeof this.Left == "string" && this.Left.indexOf("%") != -1) {
        var a = parseFloat(this.Left) * 0.01;
        b = ielt7 ? d.clientWidth * a - c.scrollWidth * a + d.scrollLeft : d.clientWidth * a - c.scrollWidth * a
    }
    c.style.top = Math.round(g) + "px";
    c.style.left = Math.round(b) + "px"
};
Dialog.setBgDivSize = function () {
    var a = $bodyDimensions(topWin);
    if (Dialog.bgDiv) {
        if (ielt7) {
            Dialog.bgDiv.style.height = a.clientHeight + "px";
            Dialog.bgDiv.style.top = a.scrollTop + "px";
            Dialog.bgDiv.childNodes[0].style.display = "none";
            Dialog.bgDiv.childNodes[0].style.display = ""
        } else {
            Dialog.bgDiv.style.height = a.scrollHeight + "px"
        }
    }
};
Dialog.resetPosition = function () {
    Dialog.setBgDivSize();
    for (var b = 0, a = topWin.Dialog._dialogArray.length; b < a; b++) {
        topWin.Dialog._dialogArray[b].setPosition()
    }
};
Dialog.prototype.create = function () {
    var h = $bodyDimensions(topWin);
    if (typeof(this.OKEvent) == "function") {
        this.ShowButtonRow = true
    }
    if (!this.Width) {
        this.Width = Math.round(h.clientWidth * 4 / 10)
    }
    if (!this.Height) {
        this.Height = Math.round(this.Width / 2)
    }
    if (this.MessageTitle || this.Message) {
        this.ShowMessageRow = true
    }
    var c = this.Width + 13 + 13;
    var g = this.Height + 33 + 13 + (this.ShowButtonRow ? 40 : 0) + (this.ShowMessageRow ? 50 : 0);
    if (c > h.clientWidth) {
        this.Width = Math.round(h.clientWidth - 26)
    }
    if (g > h.clientHeight) {
        this.Height = Math.round(h.clientHeight - 46 - (this.ShowButtonRow ? 40 : 0) - (this.ShowMessageRow ? 50 : 0))
    }
    var f = '  <table id="_DialogTable_{thisID}"  width="' + (this.Width + 26) + '" cellspacing="0" cellpadding="0" border="0" style="font-size:12px; line-height:1.4;border-collapse: collapse;border: none;background: rgba(1,43,57,0.5);padding: 2px;">' +
        '    <tr id="_Draghandle_{thisID}" onselectstart="return false;" style="-moz-user-select: -moz-none; ' + (this.Drag ? "cursor: move;" : "") + '">      <td width="13" height="33" style="background-image: url({IMAGESPATH}dialog_lt.png) !important;background: url({IMAGESPATH}dialog_lt.gif) no-repeat 0 0;"><div style="width: 13px;"></div></td>      <td height="33" style="background-image:url({IMAGESPATH}dialog_ct.png) !important;background: url({IMAGESPATH}dialog_ct.gif) repeat-x top;"><div style="padding: 9px 0 0 4px; float: left; font-weight: bold; color:#fff;"><img align="absmiddle" src="{IMAGESPATH}icon_dialog.gif"/><span id="_Title_{thisID}">' + this.Title + '</span></div>        <div id="_ButtonClose_{thisID}" onclick="fixProgress();Dialog.getInstance(\'{thisID}\').cancelButton.onclick.apply(Dialog.getInstance(\'{thisID}\').cancelButton,[]);" onmouseout="this.style.backgroundImage=\'url({IMAGESPATH}dialog_closebtn.gif)\'" onmouseover="this.style.backgroundImage=\'url({IMAGESPATH}dialog_closebtn_over.gif)\'" style="margin: 4px 0 0;*margin-top: 5px; position: relative;top:auto; cursor: pointer; float: right; height: 17px; width: 28px; background: url({IMAGESPATH}dialog_closebtn.gif) 0 0;' + (ielt7 ? "margin-top: 3px;" : "") + (this.ShowCloseButton ? "" : "display:none;") + '"></div></td>      <td width="13" height="33" style="background-image: url({IMAGESPATH}dialog_rt.png) !important;background: url({IMAGESPATH}dialog_rt.gif) no-repeat right 0;"><div style="width: 13px;"><a id="_forTab_{thisID}" href="#;"></a></div></td>    </tr>    <tr valign="top">      <td width="13" style="background-image: url({IMAGESPATH}dialog_mlm.png) !important;background: url({IMAGESPATH}dialog_mlm.gif) repeat-y left;"></td>      <td align="center"><table width="100%" cellspacing="0" cellpadding="0" border="0" bgcolor="#ffffff" ' + (this.TableBackGroundColor.length>0?' style="background-color:#fff"' :'') +'>          <tr id="_MessageRow_{thisID}" style="' + (this.ShowMessageRow ? "" : "display:none") + '">            <td valign="top" height="50"><table width="100%" cellspacing="0" cellpadding="0" border="0" style="background:#eaece9 url({IMAGESPATH}dialog_bg.jpg) no-repeat scroll right top;" id="_MessageTable_{thisID}">                <tr>                  <td width="50" height="50" align="center"><img width="32" height="32" src="{IMAGESPATH}' + this.MessageIcon + '" id="_MessageIcon_{thisID}"/></td>                  <td align="left" style="line-height: 16px;"><div id="_MessageTitle_{thisID}" style="font-weight:bold">' + this.MessageTitle + '</div>                    <div id="_Message_{thisID}">' + this.Message + '</div></td>                </tr>              </table></td>          </tr>          <tr>            <td valign="top" align="center"><div id="_Container_{thisID}" style="position: relative; width: ' + this.Width + "px; height: " + this.Height + 'px;">                <div  style="position: absolute; height: 100%; width: 100%; display: none; background-color:#fff; opacity: 0.5;" id="_Covering_{thisID}">&nbsp;</div>	' + (function (k) {
            if (k.InnerHtml) {
                return k.InnerHtml
            }
            if (k.URL) {
                return '<iframe width="100%" height="100%" frameborder="0" style="border:none 0;" id="_DialogFrame_' + k.ID + '" ' + (k.InnerFrameName ? 'name="' + k.InnerFrameName + '"' : "") + ' src="' + k.displacePath() + '"></iframe>'
            }
            return ""
        })(this) + '              </div></td>          </tr>          <tr id="_ButtonRow_{thisID}" style="' + (this.ShowButtonRow ? "" : "display:none") + '">            <td height="36"><div id="_DialogButtons_{thisID}" style="border-top: 1px solid #DADEE5; padding: 8px 20px; text-align: right; background-color:#f6f6f6;">                <input type="button" style="' + (this.ShowOkButton ? "" : "display:none") + '"  value="' + this.OkButtonText + '" id="_ButtonOK_{thisID}"/>                <input type="button" style="' + (this.ShowCancelButton ? "" : "display:none") + '"  value="' + this.CancelButtonText + '" onclick="Dialog.getInstance(\'{thisID}\').close();" id="_ButtonCancel_{thisID}"/>              </div></td>          </tr>        </table></td>      <td width="13" style="background-image: url({IMAGESPATH}dialog_mrm.png) !important;background: url({IMAGESPATH}dialog_mrm.gif) repeat-y right;"></td>    </tr>    <tr>      <td width="13" height="13" style="background-image: url({IMAGESPATH}dialog_lb.png) !important;background: url({IMAGESPATH}dialog_lb.gif) no-repeat 0 bottom;"></td>      <td style="background-image: url({IMAGESPATH}dialog_cb.png) !important;background: url({IMAGESPATH}dialog_cb.gif) repeat-x bottom;"></td>      <td width="13" height="13" style="background-image: url({IMAGESPATH}dialog_rb.png) !important;background: url({IMAGESPATH}dialog_rb.gif) no-repeat right bottom;"><a onfocus=\'$id("_forTab_{thisID}").focus();\' href="#;"></a></td>    </tr>  </table></div>';
    f = f.replace(/\{IMAGESPATH\}/gm, IMAGESPATH).replace(/\{thisID\}/gm, this.ID);
    var j = topWin.$id("_DialogDiv_" + this.ID);
    if (!j) {
        j = topDoc.createElement("div");
        $(j).draggable({ //让dialog有拖拽限制
            containParent: true
        });
        j.id = "_DialogDiv_" + this.ID;
        topDoc.getElementsByTagName("BODY")[0].appendChild(j)
    }
    if (isIE && topDoc.compatMode == "BackCompat" || ielt7) {
        j.style.position = "absolute";
        this.fixedPosition = false
    } else {
        j.style.position = "fixed";
        this.fixedPosition = true
    }
    j.style.left = "-9999px";
    j.style.top = "-9999px";
    j.innerHTML = f;
    if (this.InvokeElementId) {
        var d = $id(this.InvokeElementId);
        d.style.position = "";
        d.style.display = "";
        if (isIE) {
            var b = topDoc.createElement("div");
            b.innerHTML = d.outerHTML;
            d.outerHTML = "";
            topWin.$id("_Covering_" + this.ID).parentNode.appendChild(b)
        } else {
            topWin.$id("_Covering_" + this.ID).parentNode.appendChild(d)
        }
    }
    this.openerWindow = window;
    if (window.ownerDialog) {
        this.openerDialog = window.ownerDialog
    }
    if (this.URL) {
        if (topWin.$id("_DialogFrame_" + this.ID)) {
            this.innerFrame = topWin.$id("_DialogFrame_" + this.ID)
        }
        var a = this;
        this.innerFrameOnload = function () {
            a.innerWin = a.innerFrame.contentWindow;
            try {
                a.innerWin.ownerDialog = a;
                a.innerDoc = a.innerWin.document;
                if (a.Title == "　" && a.innerDoc && a.innerDoc.title) {
                    if (a.innerDoc.title) {
                        topWin.$id("_Title_" + a.ID).innerHTML = a.innerDoc.title
                    }
                }
            } catch (k) {
                if (window.console && window.console.log) {
                    console.log("可能存在访问限制，不能获取到浮动窗口中的文档对象。")
                }
                a.unauthorized = true
            }
            if (typeof(a.OnLoad) == "function") {
                a.OnLoad()
            }
        };
        if (!isGecko) {
            this.innerFrame.attachEvent("onreadystatechange", function () {
                if ((/loaded|complete/).test(a.innerFrame.readyState)) {
                    a.innerFrameOnload()
                }
            })
        } else {
            this.innerFrame.onload = a.innerFrameOnload
        }
    }
    topWin.$id("_DialogDiv_" + this.ID).dialogId = this.ID;
    topWin.$id("_DialogDiv_" + this.ID).dialogInstance = this;
    this.attachBehaviors();
    this.okButton = topWin.$id("_ButtonOK_" + this.ID);
    this.cancelButton = topWin.$id("_ButtonCancel_" + this.ID);
    j = null;
    $("input:button[class='']").addClass("button");
    $("input:button[class='button']").each(function () {
        $(this).hover(function () {
            $(this).removeClass("button");
            $(this).addClass("button_hover")
        }, function () {
            $(this).removeClass("button_hover");
            $(this).addClass("button")
        })
    })
};
Dialog.prototype.setSize = function (a, b) {
    if (a && +a > 20) {
        this.Width = +a;
        topWin.$id("_DialogTable_" + this.ID).width = this.Width + 26;
        topWin.$id("_Container_" + this.ID).style.width = this.Width + "px"
    }
    if (b && +b > 10) {
        this.Height = +b;
        topWin.$id("_Container_" + this.ID).style.height = this.Height + "px"
    }
    this.setPosition()
};
Dialog.prototype.show = function () {
    this.create();
    var b = Dialog.getBgdiv(),
        f = this.getDialogDiv();
    f.style.zIndex = this.zindex = parseInt(Dialog.bgDiv.style.zIndex) + 1;
    if (topWin.Dialog._dialogArray.length > 0) {
        f.style.zIndex = this.zindex = topWin.Dialog._dialogArray[topWin.Dialog._dialogArray.length - 1].zindex + 2
    } else {
        b.style.display = "none";
        if (HideScrollbar) {
            var d = topDoc.getElementsByTagName(topDoc.compatMode == "BackCompat" ? "BODY" : "HTML")[0];
            d.styleOverflow = d.style.overflow;
            if (window.navigator.userAgent.indexOf("Firefox/3.6") != -1) {
                var c = d.scrollTop;
                d.style.overflow = "hidden";
                d.scrollTop = c
            } else {
                d.style.overflow = "hidden"
            }
        }
    }
    topWin.Dialog._dialogArray.push(this);
    Dialog._childDialogArray.push(this);
    if (Dialog._childDialogArray.length == 1) {
        if (window.ownerDialog) {
            ownerDialog.hiddenCloseButton()
        }
    }
    if (this.Modal) {
        b.style.zIndex = topWin.Dialog._dialogArray[topWin.Dialog._dialogArray.length - 1].zindex - 1;
        Dialog.setBgDivSize();
        if (b.style.display == "none") {
            if (this.Animator) {
                var a = topWin.$id("_DialogBGMask");
                b.style.display = "";
                if (isIE) {} else {}
                a = null
            } else {
                b.style.display = ""
            }
        }
    }
    this.setPosition();
    if (this.CancelEvent) {
        this.cancelButton.onclick = this.CancelEvent
    }
    if (this.OKEvent) {
        this.okButton.onclick = this.OKEvent
    }
    if (this.AutoClose && this.AutoClose > 0) {
        this.autoClose()
    }
    this.opened = true;
    b = null
};
Dialog.prototype.close = function () {
    if (this.unauthorized == false) {
        if (this.innerWin && this.innerWin.Dialog && this.innerWin.Dialog._childDialogArray.length > 0) {
            return
        }
    }
    var j = this.getDialogDiv();
    if (this == topWin.Dialog._dialogArray[topWin.Dialog._dialogArray.length - 1]) {
        var h = topWin.Dialog._dialogArray.pop()
    } else {
        topWin.Dialog._dialogArray.remove(this)
    }
    Dialog._childDialogArray.remove(this);
    if (Dialog._childDialogArray.length == 0) {
        if (window.ownerDialog) {
            ownerDialog.showCloseButton()
        }
    }
    if (this.InvokeElementId) {
        var d = topWin.$id(this.InvokeElementId);
        d.style.display = "none";
        if (isIE) {
            var g = document.createElement("div");
            g.innerHTML = d.outerHTML;
            d.outerHTML = "";
            document.getElementsByTagName("BODY")[0].appendChild(g)
        } else {
            document.getElementsByTagName("BODY")[0].appendChild(d)
        }
    }
    if (topWin.Dialog._dialogArray.length > 0) {
        if (this.Modal && h) {
            var f = topWin.Dialog._dialogArray.length;
            var b = true;
            while (f) {
                --f;
                if (topWin.Dialog._dialogArray[f].Modal) {
                    Dialog.bgDiv.style.zIndex = topWin.Dialog._dialogArray[f].zindex - 1;
                    b = false;
                    break
                }
            }
            if (b) {
                Dialog.bgDiv.style.display = "none"
            }
        }
    } else {
        Dialog.bgDiv.style.zIndex = "900";
        Dialog.bgDiv.style.display = "none";
        if (HideScrollbar) {
            var c = topDoc.getElementsByTagName(topDoc.compatMode == "BackCompat" ? "BODY" : "HTML")[0];
            if (c.styleOverflow != undefined) {
                if (window.navigator.userAgent.indexOf("Firefox/3.6") != -1) {
                    var a = c.scrollTop;
                    c.style.overflow = c.styleOverflow;
                    c.scrollTop = a
                } else {
                    c.style.overflow = c.styleOverflow
                }
            }
        }
    }
    this.openerWindow.focus();
    if (isIE && !isIE8) {
        j.dialogInstance = null;
        if (this.CancelEvent) {
            this.cancelButton.onclick = null
        }
        if (this.OKEvent) {
            this.okButton.onclick = null
        }
        topWin.$id("_DialogDiv_" + this.ID).onDragStart = null;
        topWin.$id("_DialogDiv_" + this.ID).onDragEnd = null;
        topWin.$id("_Draghandle_" + this.ID).onmousedown = null;
        topWin.$id("_Draghandle_" + this.ID).root = null;
        j.outerHTML = "";
        CollectGarbage()
    } else {
        var k = topWin.$id("_RycDiv");
        if (!k) {
            k = topDoc.createElement("div");
            k.id = "_RycDiv"
        }
        k.appendChild(j);
        k.innerHTML = "";
        k = null
    }
    this.innerFrame = null;
    this.bgDiv = null;
    j = null;
    this.closed = true
};
Dialog.prototype.autoClose = function () {
    if (this.closed) {
        clearTimeout(this._closeTimeoutId);
        return
    }
    this.AutoClose -= 1;
    topWin.$id("_Title_" + this.ID).innerHTML = this.AutoClose + " 秒后自动关闭";
    if (this.AutoClose <= 0) {
        this.close()
    } else {
        var a = this;
        this._closeTimeoutId = setTimeout(function () {
            a.autoClose()
        }, 1000)
    }
};
Dialog.getInstance = function (b) {
    var a = topWin.$id("_DialogDiv_" + b);
    if (!a) {
        alert("没有取到对应ID的弹出框页面对象")
    }
    try {
        return a.dialogInstance
    } finally {
        a = null
    }
};
Dialog.prototype.addButton = function (f, a, d) {
    topWin.$id("_ButtonRow_" + this.ID).style.display = "";
    this.ShowButtonRow = true;
    var c = topDoc.createElement("input");
    c.id = "_Button_" + this.ID + "_" + f;
    c.type = "button";
    c.style.cssText = "margin-right:5px";
    c.value = a;
    c.onclick = d;
    var b = topWin.$id("_DialogButtons_" + this.ID).getElementsByTagName("INPUT")[0];
    b.parentNode.insertBefore(c, b);
    $("input:button[class='']").addClass("button");
    $("input:button[class='button']").each(function () {
        $(this).hover(function () {
            $(this).removeClass("button");
            $(this).addClass("button_hover")
        }, function () {
            $(this).removeClass("button_hover");
            $(this).addClass("button")
        })
    });
    return c
};
Dialog.prototype.removeButton = function (b) {
    var a = topWin.$id("_DialogButtons_" + this.ID).getElementsByTagName("INPUT")[0];
    a.parentNode.removeChild(b)
};
Dialog.prototype.hiddenCloseButton = function (b) {
    var a = topWin.$id("_ButtonClose_" + this.ID);
    if (a) {
        a.style.display = "none"
    }
};
Dialog.prototype.showCloseButton = function (b) {
    var a = topWin.$id("_ButtonClose_" + this.ID);
    if (a) {
        a.style.display = ""
    }
};
Dialog.getBgdiv = function () {
    if (Dialog.bgDiv) {
        return Dialog.bgDiv
    }
    var d = topWin.$id("_DialogBGDiv");
    if (!d) {
        d = topDoc.createElement("div");
        d.id = "_DialogBGDiv";
        d.style.cssText = "position:absolute;left:0px;top:0px;width:100%;height:100%;z-index:900";
        var a = '<div style="position:relative;width:100%;height:100%;">';
        var c = '<div id="_DialogBGMask" style="position:absolute;background-color:#333;opacity:0.4;filter:alpha(opacity=40);width:100%;height:100%;"></div>';
        var f = ielt7 ? '<iframe src="about:blank" style="filter:alpha(opacity=0);" width="100%" height="100%"></iframe>' : "";
        d.innerHTML = a + c + f + "</div>";
        topDoc.getElementsByTagName("BODY")[0].appendChild(d);
        if (ielt7) {
            var b = d.getElementsByTagName("IFRAME")[0].contentWindow.document;
            b.open();
            b.write("<body style='background-color:#333' oncontextmenu='return false;'></body>");
            b.close();
            b = null
        }
    }
    Dialog.bgDiv = d;
    d = null;
    return Dialog.bgDiv
};
Dialog.prototype.getDialogDiv = function () {
    var a = topWin.$id("_DialogDiv_" + this.ID);
    if (!a) {
        alert("获取弹出层页面对象出错！")
    }
    try {
        return a
    } finally {
        a = null
    }
};
Dialog.onKeyDown = function (a) {
    var a = window.event || a;
    if ((a.shiftKey && a.keyCode == 9) || a.keyCode == 8) {
        if (topWin.Dialog._dialogArray.length > 0) {
            var c = a.srcElement || a.target;
            if (c.tagName != "INPUT" && c.tagName != "TEXTAREA") {
                stopEvent(a);
                return false
            }
        }
    }
    if (a.keyCode == 27) {
        var b = topWin.Dialog._dialogArray[topWin.Dialog._dialogArray.length - 1];
        if (b.ShowCloseButton) {
            Dialog.close()
        }
    }
};
Dialog.close = function (b) {
    if (topWin.Dialog._dialogArray.length > 0) {
        var a = topWin.Dialog._dialogArray[topWin.Dialog._dialogArray.length - 1];
        if (a.MsgForESC) {
            Dialog.confirm(a.MsgForESC, function () {
                a.cancelButton.onclick.apply(a.cancelButton, [])
            })
        } else {
            a.cancelButton.onclick.apply(a.cancelButton, [])
        }
    }
};
Dialog.alert = function (f, c, a, b) {
    var a = a || 300,
        b = b || 110;
    var d = new Dialog({
        Width: a,
        Height: b
    });
    d.ShowButtonRow = true;
    d.Title = "系统提示";
    d.CancelEvent = function () {
        d.close();
        if (c) {
            c()
        }
    };
    d.InnerHtml = '<table height="100%" border="0" align="center" cellpadding="10" cellspacing="0">		<tr><td align="right"><img id="Icon_' + this.ID + '" src="' + IMAGESPATH + 'icon_alert.gif" width="34" height="34" align="absmiddle"></td>			<td align="left" id="Message_' + this.ID + '" style="font-size:9pt">' + f + "</td></tr>	</table>";
    d.show();
    d.okButton.parentNode.style.textAlign = "center";
    d.okButton.style.display = "none";
    d.cancelButton.value = d.OkButtonText
};
Dialog.confirm = function (g, c, b, a, d) {
    var a = a || 300,
        d = d || 110;
    var f = new Dialog({
        Width: a,
        Height: d
    });
    f.ShowButtonRow = true;
    f.Title = "信息确认";
    f.CancelEvent = function () {
        f.close();
        if (b) {
            b()
        }
    };
    f.OKEvent = function () {
        f.close();
        if (c) {
            c()
        }
    };
    f.InnerHtml = '<table height="100%" border="0" align="center" cellpadding="10" cellspacing="0">		<tr><td align="right"><img id="Icon_' + this.ID + '" src="' + IMAGESPATH + 'icon_query.gif" width="34" height="34" align="absmiddle"></td>			<td align="left" id="Message_' + this.ID + '" style="font-size:9pt">' + g + "</td></tr>	</table>";
    f.show();
    f.okButton.parentNode.style.textAlign = "center"
};
Dialog.open = function (a) {
    var b = new Dialog(a);
    b.show();
    return b
};
window.attachEvent("onload", Dialog.attachBehaviors);
var scripts = document.getElementsByTagName("script");
for (var i = 0; i < scripts.length; i++) {
    if (/.*zDrag\.plugins$/g.test(scripts[i].getAttribute("src"))) {
        break
    }
    if (/.*zDialog\.plugins$/g.test(scripts[i].getAttribute("src"))) {
        var jsPath = scripts[i].getAttribute("src").replace(/zDialog\.plugins$/g, "");
        document.write('<script type="text/javascript" src="' + jsPath + 'zDrag.plugins"><\/script>');
        break
    }
}
function fixProgress() {
    try {
        if (top.progressFlag == 1) {
            top.progressFlag = 0
        }
    } catch (a) {}
};

$.fn.draggable = function (opt) {
    var base = this;
    var settings = {
        handle: "",
        cursor: "move",
        axis: null,
        containParent: false
    };

    opt = $.extend(settings, opt);

    if (opt.handle === "") {
        var $el = base;
    } else {
        var $el = base.find(opt.handle);
    }



    return $el.css('cursor', opt.cursor).on("mousedown", function (e) {
        if (opt.handle === "") {
            var $drag = $(this).addClass('draggable');
        } else {
            var $drag = $(this).addClass('active-handle').parent().addClass('draggable');
        }
        var z_idx = $drag.css('z-index'),
            drg_h = $drag.outerHeight(),
            drg_w = $drag.outerWidth(),
            pos_y = $drag.offset().top + drg_h - e.pageY,
            pos_x = $drag.offset().left + drg_w - e.pageX;


        var parent = $(this).parent();

        var parW = parent.width(),
            parH = parent.height();

        var parX1 = parseInt(parent.offset().left) + parseInt(parent.css('padding-left').replace('px', '')),
            parX2 = parX1 + parW,
            parY1 = parseInt(parent.offset().top) + parseInt(parent.css('padding-top').replace('px', '')),
            parY2 = parY1 + parH;



        $drag.css('z-index', 1000).parents().on("mousemove", function (e) {
            var off_top = e.pageY + pos_y - drg_h,
                off_left = e.pageX + pos_x - drg_w,
                offst = null;

            if (opt.containParent === true) {
                if (off_left < parX1) off_left = parX1;
                if (off_left > parX2 - drg_w) off_left = parX2 - drg_w;
                if (off_top < parY1) off_top = parY1;
                if (off_top > parY2 - drg_h) off_top = parY2 - drg_h;
            }

            if (opt.axis == "x") {
                offst = {
                    left: off_left
                };
            } else if (opt.axis == "y") {
                offst = {
                    top: off_top
                };
            } else {
                offst = {
                    left: off_left,
                    top: off_top
                };
            }

            $('.draggable').offset(offst);

            $('.draggable, html').on("mouseup", function () {
                $drag.parents().off('mousemove');
                $($el).removeClass('draggable').css('z-index', z_idx);
            });

        });
        e.preventDefault(); // disable selection
    }).on("mouseup", function () {
        if (opt.handle === "") {
            $(this).removeClass('draggable');
        } else {
            $(this).removeClass('active-handle').parent().removeClass('draggable');
        }
        $el.off('mousedown', function (e) {
            e.preventDefault()
        });
    });
}

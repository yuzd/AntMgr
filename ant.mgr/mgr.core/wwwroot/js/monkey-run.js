(function (Global) {
    'use strict';

    var monkeyrun = {
        version: '0.7.3'
    };

    Global.MonkeyRun = monkeyrun;

}(this));

(function (MonkeyRun) {
    'use strict';

    /**
    * Functions for browser
    */

    var browser = {

        userAgent: navigator.userAgent,

        webkitVersion: (function () {
            var u = navigator.userAgent;
            var matchIndex = u.indexOf('AppleWebKit/');
            if (matchIndex > -1) {
                var num = u.substring(matchIndex + 12, matchIndex + 18).replace(' ', '');
                return parseFloat(num);
            }
            return '';
        })(),

        detection: (function () {
            var u = navigator.userAgent;
            return {
                trident: u.indexOf('Trident') > -1, //IE内核
                presto: u.indexOf('Presto') > -1, //opera内核
                webKit: u.indexOf('AppleWebKit') > -1, //苹果、谷歌内核
                gecko: u.indexOf('Gecko') > -1 && u.indexOf('KHTML') === -1, //火狐内核
                mobile: !!u.match(/AppleWebKit.*Mobile.*/) || !!u.match(/AppleWebKit/), //是否为移动终端
                ios: !!u.match(/\(i[^;]+;( U;)? CPU.+Mac OS X/), //ios终端
                android: u.indexOf('Android') > -1 || u.indexOf('Linux') > -1, //android终端或者uc浏览器
                iPhone: u.indexOf('iPhone') > -1 || u.indexOf('Mac') > -1, //是否为iPhone或者QQHD浏览器
                iPad: u.indexOf('iPad') > -1, //是否iPad
                weixin: u.indexOf('MicroMessenger') > -1,
                QQBrowser: u.indexOf('MQQBrowser') > -1
            };
        })(),

        language: (navigator.browserLanguage || navigator.language).toLowerCase(),

        screen: (function () {
            var width = document.documentElement.clientWidth || document.body.clientWidth;
            var height = document.documentElement.clientHeight || document.body.clientHeight;
            var ratio = width / height;
            return {
                width: width,
                height: height,
                ratio: ratio
            };
        })()
    }

    MonkeyRun.browser = browser;

}(MonkeyRun));

(function (MonkeyRun) {
    'use strict';

    /**
    * Functions for Geo
    */
    var geo = {

        /**
         * Get the distance of two points
         * @param {number} lat1 latitude of point one
         * @param {number} lng1 longitude of point one
         * @param {number} lat2 latitude of point two
         * @param {number} lng2 longitude of point two
         * @returns {number} distance
         */
        getDistance: function (lat1, lng1, lat2, lng2) {
            var b = Math.PI / 180;
            var c = Math.sin((lat2 - lat1) * b / 2);
            var d = Math.sin((lng2 - lng1) * b / 2);
            var a = c * c + d * d * Math.cos(lat1 * b) * Math.cos(lat2 * b);
            return 12756274 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
        },
    }

    MonkeyRun.geo = geo;

}(MonkeyRun));

(function (MonkeyRun) {
    'use strict';

    /**
     * 
     * @param {Object} value input
     * @returns {boolean} if input value is null or empty
     */
    function isNullOrEmpty(value) {
        return value === undefined || value === null || value === '';
    }

    /**
     * Append params to an url
     * @param {string} url source url
     * @param {Array<Object>} params params to append, each param is an object, eg: {name:'Monkey', age:'3'}, param will be removed if value is null or empty string.
     * @returns {string} result url
     */
    function appendParams(url, params) {
        if (params) {
            var baseWithSearch = url.split('#')[0];
            var hash = url.split('#')[1];
            for (var key in params) {
                var attrValue = params[key];
                var newParam = key + "=" + attrValue;
                if (baseWithSearch.indexOf('?') > 0) {
                    var oldParamReg = new RegExp(key + '=[-%.!~*\'\(\)\\w]*', 'g');

                    if (oldParamReg.test(baseWithSearch)) {
                        if (!isNullOrEmpty(attrValue)) {
                            baseWithSearch = baseWithSearch.replace(oldParamReg, newParam);
                        }
                        else {
                            var removeParamReg = new RegExp('[?&]' + key + '=[-%.!~*\'\(\)\\w]*', 'g');
                            baseWithSearch = baseWithSearch.replace(removeParamReg, '');
                        }
                    } else {
                        if (!isNullOrEmpty(attrValue)) {
                            baseWithSearch += "&" + newParam;
                        }
                    }
                } else if (!isNullOrEmpty(attrValue)) {
                    baseWithSearch += "?" + newParam;
                }
            }

            if (hash) {
                url = baseWithSearch + '#' + hash;
            } else {
                url = baseWithSearch;
            }
        }
        return url;
    }

    /**
     * Jump to a page using .net mvc
     * @param {string} controller controller name
     * @param {string} action action name
     * @param {Array<Object>} params params to append, each param is an object, eg: {key:'name', value:'Monkey'}
     */
    function mvcJump(controller, action, params) {
        if (window.appUrl) {
            //适配虚拟目录
            window.location.href = appendParams(window.location.protocol +
                '//' +
                window.location.host +
                '/' +
                window.appUrl +
                '/' +
                controller +
                '/' +
                action,
                params);
        } else {
            window.location.href = appendParams(window.location.protocol +
                '//' +
                window.location.host +
                '/' +
                controller +
                '/' +
                action,
                params);
        }
       
    }

    /**
     * Get Weixin authorize redirect url
     * @param {string} appid 公众号的唯一标识
     * @param {string} url 授权后重定向的回调链接地址（无需urlencode）
     * @param {string} scope 应用授权作用域，snsapi_base （不弹出授权页面，直接跳转，只能获取用户openid），snsapi_userinfo （弹出授权页面，可通过openid拿到昵称、性别、所在地。并且，即使在未关注的情况下，只要用户授权，也能获取其信息）
     * @param {string} state 重定向后会带上state参数，开发者可以填写a-zA-Z0-9的参数值，最多128字节
     * @returns {string} redirect url
     */
    function getWxAuthRedirectUrl(appid, url, scope, state) {
        var result = 'https://open.weixin.qq.com/connect/oauth2/authorize?appid=' + appid + '&redirect_uri=' + encodeURIComponent(url) + '&response_type=code&scope=' + scope + '&state=' + state + '#wechat_redirect';
        return result;
    }

    /**
     * Get the value of a url param
     * @param {string} key key of the param
     * @returns {string} value of the param
     */
    function getUrlParamValue(key) {
        var args = {};
        var query = window.location.search.substring(1);
        var pairs = query.split('&');
        for (var i = 0; i < pairs.length; i++) {
            var pos = pairs[i].indexOf('=');
            if (pos === -1) {
                continue;
            }
            var argname = pairs[i].substring(0, pos).toLowerCase();
            var value = pairs[i].substring(pos + 1);
            args[argname] = decodeURIComponent(value);
        }
        return args[key.toLowerCase()];
    }

    /**
     * Storage data using localStorage
     */
    var storage = {
        /**
         * Get data
         * @param {string} key key of the data
         * @param {boolean} needParse the result will be parsed if this is set true
         * @returns {Object} result
         */
        get: function (key, needParse) {
            if (key && window.localStorage) {
                var value = window.localStorage.getItem(key);
                if (needParse) {
                    value = JSON.parse(value);
                }
                return value;
            }
            return null;
        },

        /**
         * Set data
         * @param {string} key key of the data
         * @param {Object} value value of the data
         */
        set: function (key, value) {
            if (key && value && window.localStorage) {
                if (typeof value !== 'string') {
                    value = JSON.stringify(value);
                }
                window.localStorage.setItem(key, value);
            }
        },

        /**
         * Remove data
         * @param {string} key key of the data
         */
        remove: function (key) {
            if (key && window.localStorage) {
                window.localStorage.removeItem(key);
            }
        }
    };

    function checkPhone(phoneNum) {
        return (/^1[34578]\d{9}$/.test(phoneNum));
    };
    function checkEmail(email) {
        return (/^([\.a-zA-Z0-9_-])+@([a-zA-Z0-9_-])+(\.[a-zA-Z0-9_-])+/.test(email));

    };
    MonkeyRun.appendParams = appendParams;
    MonkeyRun.getWxAuthRedirectUrl = getWxAuthRedirectUrl;
    MonkeyRun.getUrlParamValue = getUrlParamValue;
    MonkeyRun.mvcJump = mvcJump;
    MonkeyRun.storage = storage;
    MonkeyRun.checkPhone = checkPhone;
    MonkeyRun.checkEmail = checkEmail;

}(MonkeyRun));
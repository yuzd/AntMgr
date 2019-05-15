/**
 * PostMessenger
 *
 * A wrapper for postMessage(), for sending data between parent/iframe or parent/popup
 *
 * See https://github.com/Taeon/PostMessenger for docs

 * The MIT License (MIT)
 *
 * Copyright (c) 2015 Patrick Fox
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */
(function (root, factory) {
    if (typeof define === "function" && define.amd) {
        define([], factory);
    } else if (typeof exports === "object") {
        module.exports = factory();
    } else {
        root.PostMessenger = factory();
    }
}(
    this,
    function () {
        /**
         * Handle in incoming message, notify all relevant listeners
         *
         * @param       object      message_event       The message object sent by remote postMessage call
         */
        var R = function (message_event) {
            if (message_event.data.indexOf('WSDK') === 0) {
                return;
            }
            var data = JSON.parse( message_event.data );
            for ( var i = 0; i < this.l.length; i++) {
                var listener = this.l[ i ];
                if
                (
                    ( listener.domain == null || listener.domain == message_event.origin || listener.domain == '*' )
                    && 
                    ( listener.namespace == null || listener.namespace == data.namespace )
                )
                {
                    if ( listener.element_selector ) {
                        for (var e = 0; e < this.e.length; e++){
                            var element = this.e[ e ];
                            try{
                                // We need the try because otherwise we might get an error...
                                // ...trying to access properties on (for example) a Window element...
                                // ...when attempting a cross-domain request
                                if ( element.nodeName == 'IFRAME' ) {
                                    element = element.contentWindow;
                                }
                            } catch( e ){
                                // We don't care
                            }
                            if ( element  == message_event.source ) {
                                listener.func( data.data, message_event );
                            }
                        }
                    } else {
                        listener.func( data.data, message_event );
                    }
                }
            }
        };
    
        /**
         * Get any type of element, list of elements or selector as an array of elements
         *
         * @param       mixed       element     Nodelist, array, jQuery object, selector string
         *
         * @return      Array
         */
        var F = function( element ){
            var elements;
            if ( element == parent ) {
                elements = [ element ];
            } else{
                var type =  Object.prototype.toString.call(element).match( /\[object (.*)\]/ )[1];
                switch ( type ) {
                    case 'Window':
                    case 'HTMLIFrameElement':
                    case 'global': // Chrome?
                    {
                        elements = [element];
                        break;
                    }
                    case 'Object':
                    {
                        // jQuery list of elements?
                        if ( typeof jQuery != 'undefined' && element instanceof jQuery ) {
                            elements = element.toArray();
                        } else {
                            // Popup window
                            elements = [element];
                        }
                        break;
                    }
                    case 'String':
                    {
                        elements = document.querySelectorAll(element);
                        break;
                    }
                }
            }
            return (elements)?elements:element;
        };
        /**
         * Return the current domain if no domain is specified
         *
         * @param       string      domain
         *
         * @return      string
         */
        var D = function( domain ){
            // If no domain is specified, set to current domain (for security)
            return ( typeof domain == 'undefined' || domain == null )?window.location.protocol + '//' + window.location.host:domain;
        };

        /**
         * The PostMessenger object
         *
         * @param       string      domain          (Optional) Only send/receive messages to elements with this domain
         * @param       mixed       element         (Optional) Only sebd/receive messages to this element (or elements)
         * @param       string      namespace       (Optional) Send with/listen to this namespace only
         */
        var P = function( domain, element, namespace ){
            this.l = []; // Listeners
            this.d = D(domain);
            this.e = F(element);
            this.n = namespace;

            var method = (window.addEventListener)?'addEventListener':'attachEvent';
            // Listen to message from child window
            var t = this;
            window[method](
                (method == "attachEvent")?'onmessage':'message',
                function(){ R.apply( t, arguments ) },
                false
            );
        }
        /**
         * Send a message
         *
         * @param       mixed       data            What to send
         * @param       string      namespace       (Optional) Send message in this namespace  (ignored if namespace is set in constructor)
         */
        P.prototype.Send = function( data, namespace ){
            for (var i = 0; i < this.e.length; i++){
                element = this.e[i];
                if ( element != parent && element.nodeName == 'IFRAME' ) {
                    element = element.contentWindow;
                }
                element.postMessage( JSON.stringify({namespace:namespace,data:data}),this.d);
            }
        }
        /**
         * Add a callback to listen for a message
         *
         * @param       function        func            Callback function
         * @param       string          namespace       (Optional) Only listen to messages in this namespace (ignored if namespace is set in constructor)
         *
         * @returns     string          A unique identifier for the listener, which can be used with RemoveListener()
         */
        P.prototype.AddListener = function( func, namespace ){
            var id = new Date().getTime().toString() + Math.random();
            this.l.push({id:id,func:func,domain:this.d,namespace:namespace,element_selector:this.e});
            return id;
        }
        /**
         * Remove a listener
         *
         * @param       string      id      The ID returned by AddListener
         */
        P.prototype.RemoveListener = function( id ){
            for ( var i = 0; i < this.l.length; i++ ) {
                if ( this.l[ i ].id == id ) {
                    this.l.splice(i, 1);
                    break;
                }
            }
        }
        return P;
    }
));


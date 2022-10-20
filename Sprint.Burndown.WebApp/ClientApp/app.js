import Vue from 'vue'
import axios from 'axios'
import router from './router'
import store from './store'
import { sync } from 'vuex-router-sync'
import { AUTH_LOGOUT } from './store/actions/auth';
import App from 'components/app-root'

var signalR = require('./dist/signalr.min.js');

Vue.config.productionTip = false;
Vue.prototype.$http = axios;
Vue.prototype.$signalR = signalR;

sync(store, router);

axios.interceptors.request.use(function (config) {
    if (store.getters.isAuthenticated) {
        const authToken = store.getters.authorizationToken;
        config.headers.common['Authorization'] = 'Bearer ' + authToken;
    }
    return config;
});

axios.interceptors.response.use(response => {
    return response;
}, error => {
    if (error.response.status === 401) {
        console.warn('The 401 response received. Logging out.');
        store.dispatch(AUTH_LOGOUT);
        router.push('/signin');
    }

    return error;
});

const app = new Vue({
    store,
    router,
    ...App
});

export {
    app,
    router,
    store
}

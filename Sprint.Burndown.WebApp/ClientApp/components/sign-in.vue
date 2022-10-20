<template>
    <div>
        <div class="column sign-in-form">
            <h2 class="ui blue image header">
                <img src="/images/chart-bar-down-blue.png" class="logo image">
                <div class="content">
                    Log-in to JIRA account
                </div>
            </h2>
            <form class="ui large form" id="signInForm" v-on:submit.prevent autocomplete="on">
                <div class="ui stacked segment">
                    <div class="field">
                        <div class="ui left icon input">
                            <i class="user icon"></i>
                            <input type="text" name="username" placeholder="Username" v-model="username">
                        </div>
                    </div>
                    <div class="field">
                        <div class="ui left icon input">
                            <i class="lock icon"></i>
                            <input type="password" name="password" placeholder="Password" v-model="password" autocomplete="password">
                        </div>
                    </div>
                    <button type="submit" class="ui fluid large primary submit button" v-bind:class="{ loading: authInProgress }" v-on:click="onSignIn">Login</button>
                </div>

            </form>
            <div class="ui icon error negative message" v-show="authFailed">
                <i class="exclamation triangle icon" style="font-size: 1.5em;"></i>
                <div class="content">
                    <p>Incorrect email address or password.</p>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
import { AUTH_REQUEST } from '../store/actions/auth';

export default {
        data: function () {
            return {
                username: '',
                password: ''
            };
        },

        computed: {
            authFailed: function () {
                return this.$store.getters.hasAuthenticationErrors
            },

            authInProgress: function () {
                return this.$store.getters.authenticationInProgress
            }
        },

        methods: {
            onSignIn: function () {
                const self = this;
                const { username, password } = this;

                self.$store.dispatch(AUTH_REQUEST, { username, password })
                    .then((response) => {
                        self.$router.push('/')
                    })
                    .catch((response) => {
                        self.password = '';
                    });
            }
        }
}
</script>

<style>
    div.negative.message .exclamation.triangle.icon {
        font-size: 1.5em;
    }
</style>

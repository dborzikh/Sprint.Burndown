<template>
    <div class="ui fixed menu">
        <div class="ui container">
            <router-link class="header item" to="/boards">
                <img class="logo company" src="/images/chart-bar-down-blue.png">
                Burn-Down Chart
            </router-link>

            <a class="right item" v-on:click="signOut" v-show="isAuthenticated">LOGOUT</a>
        </div>
    </div>
</template>

<script>
import { AUTH_LOGOUT } from '../store/actions/auth'

export default {
    data() {
        return {
        }
    },

    computed: {
        isAuthenticated: function () {
            return this.$store.getters.isAuthenticated
        }
    },

    methods: {
        signOut: function () {
            this.$store.dispatch(AUTH_LOGOUT)
                .then(() => this.$router.push('/signin'));
        }
    }
}
</script>

<style>
</style>

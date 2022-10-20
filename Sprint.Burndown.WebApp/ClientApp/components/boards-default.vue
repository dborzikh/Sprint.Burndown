<template>
    <div>
        <loading-panel />
    </div>
</template>

<script>
import router from '../router'
import { AUTH_LOGOUT } from '../store/actions/auth'

export default {
    created: function () {
        this.$http.get('/api/board/default')
            .then(response => {
                if (response.status !== 404) {
                    router.push({ name: 'board', params: { id: response.data.id } });
                }
                else {
                    this.$store.dispatch(AUTH_LOGOUT)
                        .then(() => this.$router.push('/signin'));
                }
            })
            .catch(e => {
                console.warn(e);
                this.$store.dispatch(AUTH_LOGOUT)
                    .then(() => this.$router.push('/signin'));
            });
    }
}
</script>

<style>
</style>

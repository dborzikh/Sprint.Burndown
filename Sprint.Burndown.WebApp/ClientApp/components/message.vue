<template>
    <div class="ui icon negative message" v-show="!isHidden">
        <i class="exclamation triangle icon"></i>
        <i class="close icon" v-on:click="hideMessage()"></i>
        <div class="content">
            <div class="header">
                {{title}}
            </div>
            <p>{{details}}</p>
        </div>
    </div>
</template>

<script>
export default {
    data: function () {
        return {
            isHidden: false
        };
    },

    props: ['title', 'details'],

    methods: {
        hideMessage() {
            this.isHidden = true;
        }
    }
}
</script>

<style>
</style>

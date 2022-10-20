<template>
    <div class="ui right floated search issues-search">
        <div class="ui icon input">
            <input class="prompt" type="text" placeholder="Search issues..." v-model="searchText">
            <i class="icon pointing"  v-bind:class="[ inProgress ? 'close' : 'search' ]" v-on:click="clearText()"></i>
        </div>
        <div class="results"></div>
    </div>
</template>

<script>
    import { mapGetters } from 'vuex';
    import { SEARCH_TEXT_CHANGED } from '../store/actions/viewState'

    export default {
        data() {
            return {
                searchText: ''
            }
        },

        computed: {
            inProgress() {
                return this.searchText != '';
            },

            ...mapGetters(['hasNoFilters'])
        },

        methods: {
            clearText() {
                this.searchText = '';
            }
        },

        watch: {
            hasNoFilters() {
                if (this.hasNoFilters && this.inProgress) {
                    this.clearText()
                }
            },

            searchText() {
                this.$store.commit(SEARCH_TEXT_CHANGED, this.searchText)
            },
        }
    }
</script>

<style>
    div.ui.issues-toolbar {
        margin-bottom: -25px;
    }

    .ui.pointing.dropdown .ui.menu {
        margin-left: -70%;
    }

    div.ui.issues-search {
        display: inline-block;
        float: right;
        margin-top: 4px;
        margin-right: 8px;
    }

    .ui.icon.input > i.pointing.close.icon:not(.link) {
        pointer-events: all;
        cursor: pointer;
    }
</style>

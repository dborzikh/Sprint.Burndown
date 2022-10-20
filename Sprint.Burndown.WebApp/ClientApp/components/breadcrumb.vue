<template>
    <div class="ui breadcrumb left floated">
        <router-link class="section" to="/boards">{{rootLevel}}</router-link>
        <div class="divider"> / </div>
        <router-link class="section" :to="'/board/' + boardId">{{secondLevel}}</router-link>
        <div class="divider"> / </div>
        <div class="section" v-if="hasAdditionalLevel">
            <router-link class="section" :to="'/sprint/' + sprintId">{{firstLevel}}</router-link>
            <div class="divider"> / </div>
        </div>
        <div class="active section" v-else>{{firstLevel}}</div>
        <div class="active section" v-if="hasAdditionalLevel">{{showLevel}}</div>
    </div>
</template>

<script>
    export default {
        props: ['showLevel'],

        data: function () {
            return {
                rootLevel: 'All Boards'
            };
        },

        computed: {
            hasAdditionalLevel() {
                return this.showLevel != undefined;
            },

            firstLevel() {
                return this.$store.getters.getSprintSummary.sprintName;
            },

            secondLevel() {
                return this.$store.getters.getSprintSummary.boardName;
            },

            boardId() {
                return this.$store.getters.getSprintSummary.boardId;
            },

            sprintId() {
                return this.$store.getters.getSprintSummary.sprintId;
            }
        }
    }
</script>

<style>
    .ui.breadcrumb.left.floated {
        margin-top: 10px;
    }
</style>

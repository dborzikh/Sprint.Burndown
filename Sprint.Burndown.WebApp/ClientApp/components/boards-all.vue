<template>
    <div class="component-content">
        <loading-panel v-bind:visible="loadingInProgress" />

        <div v-show="!loadingInProgress">
            <div class="ui form">
                <div class="inline fields">
                    <div class="field">
                        <label style="margin-left: 16px">All Boards</label>
                    </div>

                    <page-menu v-on:reloadView="reloadData()" style="margin-left: auto"/>
                </div>
            </div>

            <message v-show="message.hasErrors" v-bind:title="message.title" v-bind:details="message.details"></message>

            <div class="ui center cards" style="min-width: 90vw;">
                <board-card v-for="board in boards"
                    v-on:boardClick="goToBoard(board.id)"
                    v-bind:board="board" />
            </div>
        </div>
    </div>
</template>

<script>
import router from '../router'

export default {
    data: function () {
        return {
            loadingInProgress: true,
            message: {},
            boards: null,
        };
    },

    created() {
        this.refreshData();
    },

    methods: {
        goToBoard(boardId) {
            router.push({ name: 'board', params: { id: boardId } });
        },

        reloadData() {
            this.refreshData(true);
        },

        refreshData(invalidateCache) {
            const self = this;
            invalidateCache = !!invalidateCache;

            var boardProjection = (value) => {
                return {
                    id: value.id,
                    self: value.self,
                    name: value.name
                };
            };

            self.loadingInProgress = true;

            var cacheHeaders = {
                'Cache-Control': invalidateCache ? 'no-cache' : 'only-if-cached'
            };

            self.$http.get('/api/board/getAll',  {
                    headers: cacheHeaders
                })
                .then(response => {
                    self.boards = response.data.map(boardProjection);
                    self.loadingInProgress = false;
                    self.message = {
                        hasErrors: response.data.hasIncompleteData,
                        title: 'JIRA response in incomplete',
                        details: 'Only first page of the board data was received',
                    }
                })
                .catch(e => {
                    console.warn(e);
                    self.message = {
                        hasErrors: true,
                        title: 'Unable to get JIRA response',
                        details: 'Getting the JIRA boards failed on server',
                    }

                    self.loadingInProgress = false;
                });
        }
    }
}
</script>

<style>
</style>

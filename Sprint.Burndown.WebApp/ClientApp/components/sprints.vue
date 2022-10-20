<template>
    <div class="component-content">
        <loading-panel v-bind:visible="loadingInProgress" />

        <div v-show="!loadingInProgress">
            <div class="ui form">
                <div class="inline fields">
                    <div class="field current-board">
                        <label>Current Board</label>
                    </div>
                    <div class="three wide field">
                        <div class="ui fluid selection dropdown" id="currentBoard" v-bind:class="{ loading: loadingInProgress, disabled: loadingInProgress }">
                            <input type="hidden" v-model="currentBoard">
                            <i class="dropdown icon"></i>
                            <div class="text">Loading Scrum Boards...</div>
                            <div class="menu">
                                <div class="item" :data-value="board.id" v-for="board in boards">
                                    {{board.name}}
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="ui left floated main menu">
                        <a class="undo popup icon item inline-button" data-content="Open Boards" v-on:click="goAllBoards()">
                            <i class="th icon"></i>
                        </a>
                    </div>

                    <div class="ui right floated search" style="float: right; display: inline-block; margin-top: 4px; margin-right: 8px; margin-left: auto;">
                        <div class="ui icon input">
                            <input class="prompt" type="text" placeholder="Search sprints..." style="border-radius: 500rem;" v-model="searchPattern">
                            <i class="search icon"></i>
                        </div>
                        <div class="results"></div>
                    </div>

                    <page-menu v-on:reloadView="reloadData()"/>

                </div>
            </div>

            <message v-show="message.hasErrors" v-bind:title="message.title" v-bind:details="message.details"></message>

            <div class="ui secondary pointing menu">
                <a class="item" v-show="isSearchInProgress " v-bind:class="{ active: isSearchInProgress }">
                    Search Results
                </a>
                <a class="item" v-show="!isSearchInProgress " v-bind:class="{ active: isFavorites }" v-on:click="showSprints('favorites')">
                    Favorites
                </a>
                <a class="item" v-show="!isSearchInProgress " v-bind:class="{ active: isActiveSprints }" v-on:click="showSprints('active')">
                    Active sprints
                </a>
                <a class="item" v-show="!isSearchInProgress " v-bind:class="{ active: isFutureSprints }" v-on:click="showSprints('future')">
                    Future sprints
                </a>
                <a class="item" v-show="!isSearchInProgress " v-bind:class="{ active: isStories }" v-on:click="showSprints('stories')">
                    Stories
                </a>
            </div>

            <div class="ui center cards">
                <div class="ui raised link card add-story" v-on:click="addStory()" v-if="isStories">
                    <div class="ui attached button">
                        <div class="header">
                            <i class="add icon"></i>
                            Add Story
                        </div>
                    </div>
                </div>
                <sprint-card v-for="sprint in sprints"
                    v-on:sprintClick="goToSprint(sprint.id, $event)"
                    v-bind:sprint="sprint" />
            </div>

        </div>
    </div>
</template>

<script>
import router from '../router'
import { mapGetters, mapActions } from 'vuex';
import SprintConst from '../constants'

export default {
    data: function () {
        return {
            currentBoard: '-1',
            loadingInProgress: true,
            message: {},
            boards: null,
            sprintType: SprintConst.Active,
            searchPattern: '',
            isSearchInProgress: false,
            searchedSprintType: SprintConst.Favorites,
            allSprints: [],
            sprints: []
        };
    },

    computed: {
        isActiveSprints: function () {
            return this.sprintType == SprintConst.Active && this.searchPattern === '';
        },

        isFavorites: function () {
            return this.sprintType == SprintConst.Favorites && this.searchPattern === '';
        },

        isFutureSprints: function () {
            return this.sprintType == SprintConst.Future && this.searchPattern === '';
        },

        isStories: function () {
            return this.sprintType == SprintConst.Stories && this.searchPattern === '';
        }
    },

    watch: {
        currentBoard: function (val) {
            this.$http.post('/api/board/' + val);
            router.push({ name: 'board', params: { id: val } });
        },

        searchPattern: function () {
            const isSearchNow = this.searchPattern !== '';
            const isSearchStarted = !this.isSearchInProgress && isSearchNow;
            const isSearchFinished = this.isSearchInProgress && !isSearchNow;

            if (isSearchStarted) {
                this.searchedSprintType = this.sprintType;
                this.showSprints(SprintConst.SearchResults);
            }
            else if (isSearchFinished) {
                this.showSprints(this.searchedSprintType);
            }
            else if (isSearchNow) {
                this.showSprints(SprintConst.SearchResults);
            }

            this.isSearchInProgress = isSearchNow;
        },

        $route: function (val) {
            this.refreshData();
        }
    },

    created() {
        this.refreshData();
    },

    mounted () {
    },

    updated() {
        //$('.cards .ui.progress').progress();
    },

    methods: {
        goToSprint(sprintId, event) {
            router.push({ name: 'sprint', params: { id: sprintId } });
        },

        goAllBoards() {
            router.push({ name: 'boardsAll' });
        },

        getFavorites() {
            return this.allSprints
                .filter(function (sprint) {
                    return sprint.isFavorite;
                });
        },

        getActiveSprints() {
            return this.allSprints
                .filter(function (sprint) {
                    return sprint.state === SprintConst.Active;
                });
        },

        getFutureSprints() {
            return this.allSprints
                .filter(function (sprint) {
                    return sprint.state === SprintConst.Future;
                });
        },

        getSearchResults() {
            const searchPattern = this.searchPattern;
            return this.allSprints
                .filter(function (sprint) {
                    return sprint.name.includes(searchPattern);
                });
        },

        showSprints(selectedSprintType) {
            this.sprintType = selectedSprintType;

            switch (this.sprintType) {
                case SprintConst.Favorites:
                    this.sprints = this.getFavorites();
                    break;

                case SprintConst.Active:
                    this.sprints = this.getActiveSprints();
                    break;

                case SprintConst.Future:
                    this.sprints = this.getFutureSprints();
                    break;

                case SprintConst.SearchResults:
                    this.sprints = this.getSearchResults();
                    break;

                case SprintConst.Stories:
                default:
                    this.sprints = [];
            }
        },

        reloadData() {
            this.refreshData(true);
        },

        refreshData(invalidateCache) {
            const self = this;
            const boardId = this.$route.params.id;
            invalidateCache = !!invalidateCache;

            var boardProjection = (value) => {
                return {
                    id: value.id,
                    self: value.self,
                    name: value.name,
                    type: value.type
                };
            };

            var sprintProjection = (value) => {
                return {
                    id: value.id,
                    self: value.self,
                    name: value.name,
                    type: value.type,
                    state: value.state,
                    startDate: moment(value.startDate).format('D/MMM/YYYY'),
                    endDate: moment(value.endDate).format('D/MMM/YYYY'),
                    actualMonth: value.actualMonth,
                    isFavorite: value.isFavorite,
                    percentPassed: value.percentPassed,
                    chartPreview: value.chartPreview
                };
            };

            self.loadingInProgress = true;
            var cacheHeaders = {
                'Cache-Control': invalidateCache ? 'no-cache' : 'only-if-cached'
            };

            self.$http.get('/api/board/' + boardId,
                {
                    params: {
                        reload: invalidateCache
                    },

                    headers: cacheHeaders
                })
                .then(response => {
                    const boardsNeedUpdate = !self.boards;

                    if (boardsNeedUpdate) {
                        self.boards = response.data.boards.map(boardProjection);
                    }

                    self.allSprints = response.data.sprints
                        .map(sprintProjection);

                    self.showSprints(SprintConst.Favorites);
                    self.loadingInProgress = false;

                    self.message = {
                        hasErrors: response.data.hasIncompleteData,
                        title: 'JIRA response in incomplete',
                        details: 'Only first page of the issue data was received',
                    }

                    if (boardsNeedUpdate) {
                        setTimeout(() => {
                            $('#currentBoard').dropdown('set selected', boardId);

                            $('#currentBoard').dropdown({
                                transition: 'drop',
                                onChange: function (value, text, $choice) {
                                    self.currentBoard = value;
                                }
                            });

                        },
                        100);
                    }
                })
                .catch(e => {
                    console.warn(e);
                });
        }
    }
}
</script>

<style>
    .ui.form .current-board label {
        margin-left: 16px
    }

    .ui.main.menu .inline-button {
        padding: 6px 13px 4px 13px;
    }

    .ui.cards {
        min-width: 90vw;
    }

    .ui.cards .add-story {
        min-height: 112px;
    }

    .ui.cards .add-story .button {
        min-height: 100% !important;
        padding-top: 16%;
        background-color: rgb(238,240,242);
    }
</style>

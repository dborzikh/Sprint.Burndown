<template>
    <div class="ui raised link card" :id="'sprint-card-'+sprint.id" v-on:click="$emit('sprintClick', sprint.id)">
        <div class="content">
            <div class="left floated header" :data-tooltip="sprint.name">{{sprint.name}}</div>
            <i class="right floated star icon" v-bind:class="{ active: sprint.isFavorite }" v-on:click="toggleFavorite($event)"></i>
        </div>
        <div class="content">
            <!--<span class="pull-right month-label">{{sprint.actualMonth}}</span>-->
            <div style="height: 120px;" v-html="sprint.chartPreview"></div>
        </div>
        <div class="content">
            <div class="ui teal progress" :id="'sprint-progress-'+sprint.id" style="margin-bottom: 0; height: 8px;" :title="sprintPeriod">
                <div class="small bar" style="height: 8px;" v-show="progressVisible">
                </div>
            </div>                    
        </div>
    </div>
</template>

<script>
export default {
    props: ['sprint'],

    mounted() {
        const self = this;
        const percents = self.sprint.percentPassed;
        $('#sprint-progress-' + self.sprint.id).progress({ percent: percents == 100 ? 0 : percents });
        $('#sprint-card-' + self.sprint.id + ' .header').popup(); // '.ui div[data-tooltip]'
    },

    computed: {
        sprintPeriod: function () {
            return this.sprint.startDate + ' - ' + this.sprint.endDate;
        },
        progressVisible: function () {
            return this.sprint.percentPassed < 100;
        }
    },

    methods: {
        toggleFavorite(event) {
            const self = this;

            const sprintId = self.sprint.id;
            const newIsFavorite = !self.sprint.isFavorite;

            this.$parent.$http
                .put('/api/sprint/' + sprintId + '/toggleFavorite', {
                    id: sprintId,
                    isFavorite: newIsFavorite
                })
                .then(response => {
                    $(event.srcElement).toggleClass('active', response.isFavorite);
                    self.sprint.isFavorite = response.isFavorite;
                })
                .catch(e => {
                    console.warn(e);
                });

            event.stopPropagation();
        }
    }
}
</script>

<style>
    .ui.card .header {
        overflow: hidden;
        max-height: 22px;
        max-width: 240px;
        word-break: break-all;
    }

    .ui.card .month-label {
        position: absolute;
        right: 20px;
        z-index: 101;
        top: 52px;
        color: #ccc;
    }
</style>

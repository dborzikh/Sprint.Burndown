<template>
    <div class="ui segment sprint-gantt">
        <div class="ui large active loader" v-show="inProgress"></div>
        <div id="ganttChart"></div>
    </div>
</template>

<script>
import { mapGetters } from 'vuex';
import estimateService from '../estimateService'
import { REFRESH_GANTT, SET_SPRINT_DATES } from '../store/actions/viewState'

export default {
    props: [],

    data: function () {
        return {
        };
    },

    computed: {
        holidays() {
            return this.$store.getters.calendarHolidays;
        },

        inProgress() {
            return this.$store.getters.calendarInProgress;
        },

        sprintSummary() {
            return this.$store.getters.getSprintSummary;
            
        }
    },

    watch: {
        holidays() {
            if (this.holidays.length > 0) {
                this.refreshGanttChart();
            }
        }
    },

    methods: {
        refreshGanttChart() {
            const self = this;
            const allHolidays = self.holidays.map((value) => value.getTime());

            const isHoliday = function (date) {
                const currentTime = date.getTime();
                return allHolidays.findIndex((element) => element == currentTime) !== -1;
            };

            var tasks = [
                {
                    id: 'development',
                    name: 'Development',
                    start: moment(self.sprintSummary.developBeginDate, 'D/MMM/YYYY').toDate(),
                    end: moment(self.sprintSummary.developEndDate, 'D/MMM/YYYY').toDate(),
                    progress: 0,
                    custom_class: 'bar-milestone', // optional_lay
                    category: 'develop'
                },
                {
                    id: 'testing',
                    name: 'Testing',
                    start: moment(self.sprintSummary.testingBeginDate, 'D/MMM/YYYY').toDate(),
                    end: moment(self.sprintSummary.testingEndDate, 'D/MMM/YYYY').toDate(),
                    progress: 0,
                    custom_class: 'bar-milestone',
                    category: 'testing'
                },
                {
                    id: 'regression',
                    name: 'Regression testing',
                    start: moment(self.sprintSummary.regressionBeginDate, 'D/MMM/YYYY').toDate(),
                    end: moment(self.sprintSummary.regressionEndDate, 'D/MMM/YYYY').toDate(),
                    progress: 0,
                    custom_class: 'bar-milestone',
                    category: 'regression'
                }

            ];

            const todayDate = (new Date()).setHours(0, 0, 0, 0);

            var gantt = new Gantt("#ganttChart", tasks, {
                header_height: 50,
                column_width: 30,
                step: 24,
                view_modes: ['Quarter Day', 'Half Day', 'Day', 'Week', 'Month'],
                bar_height: 20,
                bar_corner_radius: 3,
                arrow_curve: 5,
                padding: 18,
                view_mode: 'Day',
                date_format: 'YYYY-MM-DD',
                custom_popup_html: null,

                on_date_change: function (task, date1, date2) {
                    self.$store.dispatch(SET_SPRINT_DATES, {
                        category: task.category,
                        beginDate: date1,
                        endDate: date2
                    })
                },

                holidays: function (date) {
                    const currentDate = date.getTime();
                    const day = date.getDay();
                    const weekend = (day === 6) || (day === 0);
                    const holiday = isHoliday(date);

                    let result =
                        (todayDate == currentDate ? 'gantt-today' : '') +
                        (weekend ? ' gantt-weekend' : '') +
                        (holiday ? ' gantt-holiday' : '');

                    return result;
                }
            });
        },
    }
}
</script>

<style>
    .ui.segment.sprint-gantt {
        min-height: 250px;
    }
    .ui.segment.sprint-gantt #ganttChart {
        max-height: 220px;
        height: 220px;
        overflow: hidden;
    }

    #ganttChart .loader {
        margin: auto;
        display: block;
        margin-top: 8%;
    }

    #ganttChart .gantt-weekend,
    #ganttChart .gantt-holiday {
        fill: #e40000;
    }

    #ganttChart .gantt-today {
        fill: #4bb137;
    }
</style>

<template>
    <div class="ui segment sprint-summary">
        <table class="ui very basic collapsing celled table">
            <thead>
                <tr>
                    <th style="min-width: 6em;">&nbsp;</th>
                    <th>Development</th>
                    <th>Feature Testing</th>
                    <th>Regression Testing</th>
                </tr>
            </thead>

            <tbody>
                <tr>
                    <td>Start Date</td>
                    <td id="developBeginDateCell">{{sprintInfo.developBeginDate}}</td>
                    <td id="testingBeginDateCell">{{sprintInfo.testingBeginDate}}</td>
                    <td id="regressionBeginDateCell">{{sprintInfo.regressionBeginDate}}</td>
                </tr>
                <tr>
                    <td>End Date</td>
                    <td id="developEndDateCell">{{sprintInfo.developEndDate}}</td>
                    <td id="testingEndDateCell">{{sprintInfo.testingEndDate}}</td>
                    <td id="regressionEndDateCell">{{sprintInfo.regressionEndDate}}</td>
                </tr>
                <tr>
                    <td>Work Days</td>
                    <td id="developWorkDaysCell">{{sprintInfo.developWorkDays.length}}</td>
                    <td id="testingWorkDaysCell">{{sprintInfo.testingWorkDays.length}}</td>
                    <td id="regressionWorkDaysCell">{{sprintInfo.regressionWorkDays.length}}</td>
                </tr>
                <tr>
                    <td>Velocity</td>
                    <td id="developVelocity">{{sprintInfo.developVelocity}}</td>
                    <td id="testingVelocity">{{sprintInfo.testingVelocity}}</td>
                    <td></td>
                </tr>
            </tbody>
        </table>
    </div>
</template>

<script>
import { mapGetters } from 'vuex';
import estimateService from '../estimateService'

export default {
    props: [],

    data: function () {
        return {
        };
    },

    mounted() {
    },

    computed: {
        sprintInfo() {
            return this.$store.getters.getSprintSummary;
        }
    },

    watch: {
        sprintInfo(oldValue, newValue) {
            let updatedCells = [];
            const dateNames = [
                'developBeginDate', 'developEndDate', 
                'testingBeginDate', 'testingEndDate', 
                'regressionBeginDate', 'regressionEndDate'];
            
            for (const dateName of dateNames) {
                if (oldValue[dateName] !== newValue[dateName]) {
                    updatedCells.push('#' + dateName + 'Cell')
                }
            }

            for (const cellName of updatedCells) {
                $(cellName).transition('glow');
            }
        }
    },

    methods: {
    }
}
</script>

<style>
    .ui.segment.issues-summary {
        min-height: 250px;
    }
</style>

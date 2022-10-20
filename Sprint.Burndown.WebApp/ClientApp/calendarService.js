import Vue from 'vue'
import moment from 'moment'
import $store from './store'

export default {
    SECONDS_IN_HOUR: 3600,

    holidays: [],

    getHolidays() {
        return $store.getters.calendarHolidays;
    },

    toStringEstimate(estimateInSeconds) {
        let hours = Math.round(estimateInSeconds / this.SECONDS_IN_HOUR);
        let minutes = Math.round((estimateInSeconds - (hours * this.SECONDS_IN_HOUR)) / 60);

        return `${hours}h` + (minutes > 0 ? ` ${minutes}m` : '');
    },

    isWorkDay(date) {
        const currentTime = date.getTime();
        return this.holidays.findIndex((element) => element === currentTime) === -1;
    },

    getWorkDays(beginDate, endDate) {
        const self = this;

        // TODO should work manual updating
        this.holidays = this.getHolidays().map((value) => value.getTime());

        let workDays = [];
        for (var day = beginDate; day <= endDate; day.setDate(day.getDate() + 1)) {
            if (self.isWorkDay(day)) {
                workDays.push(new Date(day));
            }
        }

        return workDays;
    },

    getVelocity(beginDate, endDate, oneDayVelocity) {
        const workDays = this.getWorkDays(beginDate, endDate);

        return workDays.length * oneDayVelocity;
    }
}
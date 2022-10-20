<template>
    <div class="issues-view-switch">
        <div class="ui basic buttons">
            <div class="ui button" v-bind:class="{ active: hasIndicators }" v-on:click="showIndicators()">Indicators</div>
            <div class="ui button" v-bind:class="{ active: hasEstimates }" v-on:click="showEstimates()">Estimates</div>
            <div class="ui button" v-bind:class="{ active: hasFeatures }" v-on:click="showFeatures()">Features</div>
            <div class="ui button" v-bind:class="{ active: hasVersions }" v-on:click="showVersions()">Versions</div>
            <div class="ui button" v-bind:class="{ active: hasReporters }" v-on:click="showReporters()">Reporters</div>
        </div>
    </div>
</template>

<script>

import { mapGetters } from 'vuex';
import { DISPLAY_INDICATORS, DISPLAY_ESTIMATES, DISPLAY_FEATURES, DISPLAY_VERSIONS, DISPLAY_REPORTERS } from '../store/actions/viewState'

export default {
    data() {
        return {
        }
    },

    computed: {
        ...mapGetters([
            'hasIndicators',
            'hasEstimates',
            'hasFeatures',
            'hasVersions',
            'hasReporters'
        ])
    },

    methods: {
        showIndicators() {
            this.$store.commit(DISPLAY_INDICATORS);
        },

        showEstimates() {
            this.$store.commit(DISPLAY_ESTIMATES);
        },

        showFeatures() {
            this.$store.commit(DISPLAY_FEATURES);
        },

        showVersions() {
            this.$store.commit(DISPLAY_VERSIONS);
        },

        showReporters() {
            this.$store.commit(DISPLAY_REPORTERS);
        }
    },
}
</script>

<style>
    .issues-view-switch {
        display: inline-block;
        margin-top: 16px;
        padding-top: 0 !important;
        padding-bottom: 0 !important;
        width: auto;
    }
</style>

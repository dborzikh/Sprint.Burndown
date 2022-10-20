import Vue from 'vue'

export default {
    convertToBarSeries(dataMap) {
        let seriesData = [];

        dataMap.forEach((value, key, map) => {
            if (value > 0) {
                seriesData.push({
                    name: key,
                    data: [value]
                });
            }
        });

        return seriesData;
    }
}
const path = require('path');
const webpack = require('webpack');
const ExtractTextPlugin = require('extract-text-webpack-plugin');
const CopyWebpackPlugin = require('copy-webpack-plugin');
const bundleOutputDir = './wwwroot/dist';

module.exports = (env) => {
    const isDevBuild = !(env && env.prod);
    return [{
        stats: { modules: false },
        entry: { 'main': './ClientApp/boot-app.js' },
        resolve: {
            extensions: ['.js', '.vue'],
            alias: {
                'vue$': 'vue/dist/vue',
                'components': path.resolve(__dirname, './ClientApp/components'),
                'views': path.resolve(__dirname, './ClientApp/views'),
                'utils': path.resolve(__dirname, './ClientApp/utils'),
                'api': path.resolve(__dirname, './ClientApp/store/api')
            }
        },
        output: {
            path: path.join(__dirname, bundleOutputDir),
            filename: '[name].js',
            publicPath: '/dist/'
        },
        module: {
            rules: [
                { test: /\.vue$/, include: /ClientApp/, use: 'vue-loader' },
                { test: /\.js$/, include: /ClientApp/, use: 'babel-loader' },
                { test: /\.css$/, use: isDevBuild ? ['style-loader', 'css-loader'] : ExtractTextPlugin.extract({ use: 'css-loader' }) },
                { test: /\.(png|jpg|jpeg|gif|svg)$/, use: 'url-loader?limit=25000' }
            ]
        },
        plugins: [
            new webpack.DllReferencePlugin({
                context: __dirname,
                manifest: require('./wwwroot/dist/vendor-manifest.json')
            }),

            new CopyWebpackPlugin([
                { from: './bower_components/jquery/dist/jquery.min.js', to: 'jquery.min.js' },
                { from: './bower_components/semantic/dist/semantic.min.css', to: 'semantic.min.css' },
                { from: './bower_components/semantic/dist/semantic.min.js', to: 'semantic.min.js' },
                { from: './bower_components/semantic/dist/themes/default/assets/fonts/icons.woff2', to: 'themes/default/assets/fonts/icons.woff2' },
                { from: './bower_components/semantic/dist/themes/default/assets/fonts/icons.woff', to: 'themes/default/assets/fonts/icons.woff' },
                { from: './bower_components/semantic/dist/themes/default/assets/fonts/icons.ttf', to: 'themes/default/assets/fonts/icons.ttf' },
               
                { from: './bower_components/webix/codebase/webix.css', to: 'webix.css' },
                { from: './bower_components/webix/codebase/webix.js', to: 'webix.js' },
                { from: './bower_components/highcharts/css/highcharts.css', to: 'highcharts.css' },
                { from: './bower_components/highcharts/highcharts.js', to: 'highcharts.js' },
                { from: './ClientApp/images/app-logo.png', to: '../images/app-logo.png' },

                { from: './bower_components/webix/codebase/fonts/PTS-webfont.woff', to: 'fonts/PTS-webfont.woff' },
                { from: './bower_components/webix/codebase/fonts/PTS-bold.woff', to: 'fonts/PTS-bold.woff' },
                { from: './bower_components/webix/codebase/fonts/fontawesome-webfont.woff2', to: 'fonts/fontawesome-webfont.woff2' },
                { from: './bower_components/webix/codebase/fonts/fontawesome-webfont.woff', to: 'fonts/fontawesome-webfont.woff' },
                { from: './bower_components/webix/codebase/fonts/fontawesome-webfont.ttf', to: 'fonts/fontawesome-webfont.ttf' },
                { from: './bower_components/webix/codebase/fonts/fontawesome-webfont.eot', to: 'fonts/fontawesome-webfont.eot' },
                { from: './bower_components/webix/codebase/fonts/fontawesome-webfont.svg', to: 'fonts/fontawesome-webfont.svg' },

                { from: './node_modules/moment/moment.js', to: 'moment.js' },
                { from: './node_modules/@aspnet/signalr/dist/browser/signalr.min.js', to: 'signalr.min.js' },

                { from: './ClientApp/css/site.css', to: 'site.min.css' }
            ], { debug: true })

        ].concat(isDevBuild ? [
            // Plugins that apply in development builds only
            new webpack.SourceMapDevToolPlugin({
                filename: '[file].map', // Remove this line if you prefer inline source maps
                moduleFilenameTemplate: path.relative(bundleOutputDir, '[resourcePath]') // Point sourcemap entries to the original file locations on disk
            })
        ] : [
                // Plugins that apply in production builds only
                new webpack.optimize.UglifyJsPlugin(),
                new ExtractTextPlugin('site.css')
            ])
    }];
};

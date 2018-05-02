const path = require('path');
const webpack = require('webpack');
const ExtractTextPlugin = require('extract-text-webpack-plugin');
const CheckerPlugin = require('awesome-typescript-loader').CheckerPlugin;
const merge = require('webpack-merge');

module.exports = (env) => {
	const isDevBuild = !(env && env.prod);
	// Configuration in common to both client-side and server-side bundles
	const sharedConfig = () => ({
		stats: { modules: false },
		resolve: { extensions: ['.js', '.jsx', '.ts', '.tsx'] },
		output: {
			filename: '[name].js',
			publicPath: 'dist/' // Webpack dev middleware, if enabled, handles requests for this URL prefix
		},
		module: {
			rules: [
				{ test: /\.tsx?$/, include: /ClientApp/, use: 'awesome-typescript-loader?silent=true' },
				{ test: /\.(png|jpg|jpeg|gif|svg)$/, use: 'url-loader?limit=25000' }
			]
		},
		plugins: [new CheckerPlugin()]
	});

	const extractSass = new ExtractTextPlugin({
		filename: "[name].[contenthash].css",
		disable: process.env.NODE_ENV === "development"
	});

	// Configuration for client-side bundle suitable for running in browsers
	const clientBundleOutputDir = './wwwroot/dist';
	const clientBundleConfig = merge(sharedConfig(), {
		entry: { 'main': './ClientApp/boot.tsx' },
		module: {
			rules: [
				{
					test: /\.scss$/,
					use: extractSass.extract({
						use: [{
							loader: "css-loader"
						}, {
							loader: "sass-loader"
						}],
						// use style-loader in development
						fallback: "style-loader"
					})
				}
			]
		},
		output: { path: path.join(__dirname, clientBundleOutputDir) },
		plugins: [
			new webpack.DllReferencePlugin({
				context: __dirname,
				manifest: require('./wwwroot/dist/vendor-manifest.json')
			}),
			extractSass
		].concat(isDevBuild ? [
			// Plugins that apply in development builds only
			new webpack.SourceMapDevToolPlugin({
				filename: '[file].map', // Remove this line if you prefer inline source maps
				moduleFilenameTemplate: path.relative(clientBundleOutputDir, '[resourcePath]') // Point sourcemap entries to the original file locations on disk
			})
		] : [
				// Plugins that apply in production builds only
				new webpack.optimize.UglifyJsPlugin()
			])
	});

    //const isDevBuild = true;
	return [clientBundleConfig];
};
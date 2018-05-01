import * as React from 'react'; 

interface IFeatureProperties {
	name: string,
	renderType : string
}

interface IFeatureState {

}

export default class Feature extends
React.Component<IFeatureProperties, IFeatureState> {
	render() {
		if (this.props.renderType === "table") {
			return <tr>
				       <td>
					       {this.props.name}
				       </td>
				       Foo Data
				       <td>
				       </td>
			       </tr>;
		} else {
			return <tr> Not a table {this.props.name}
					
			       </tr>;
		}
	}
}
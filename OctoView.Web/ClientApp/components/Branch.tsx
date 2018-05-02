import * as React from 'react';
import {IPullRequest, PullRequest } from './PullRequest'

export interface IBranch {
	repo: string;
	branchName: string;
	layout: string;
	pulls: IPullRequest[];
}

export interface IBranchState {
	isOpen: boolean;
}

/*export const Feature: React.SFC<IFeature> = (props) => {
	if (props.layout === "table") {
		return <div>
			       <div>{props.repo}</div>
		       </div>;
	}
	return <div>
		       <div>{props.repo}</div>
	       </div>;
};*/

export class Branch extends
	React.Component<IBranch, any> {
	componentWillMount() {
		this.setState({ isOpen: false });
	}
	toggleState(disableIfOpen: boolean) {
		if (this.state.isOpen && disableIfOpen) {
			return;
		}
		if (this.props.pulls.length !== 0) {
			this.setState({ isOpen: !this.state.isOpen });
		} else {
			this.setState({ isOpen: false });
		}
	}
	render() {
		
		let className = 'ov-c-branch';
		if (this.state.isOpen) {
		  className += ' ov-c-branch--opened';
		}
		if (this.props.pulls.length === 0) {
			className += ' ov-c-branch--fixed';
		}		

		if (this.props.layout === 'table') {
			return <div>
				       <div>{this.props.repo}</div>
			       </div>;
		}
		return <div className={className}>
					<div className="ov-c-branch__left-indicator"></div>
			    	<div className="ov-c-branch__name" onClick={() => this.toggleState(false)}>{this.props.branchName}</div>
					<div className="ov-c-branch__approval-indicator" onClick={() => this.toggleState(false)}>
						<span className="ov-c-branch__approval-badge">0 / {this.props.pulls.length}</span>
					</div>
			    	<div className="ov-c-branch__pull-request-drawer" onClick={() => this.toggleState(true)}>
						<div className="ov-c-branch__pull-request-drawer-contents">
							{this.props.pulls.map(pulls => <PullRequest {...pulls}/>)}
						</div>
			    	</div>
		       </div>;
	}
}
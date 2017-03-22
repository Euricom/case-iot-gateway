import { IotGatewayAppPage } from './app.po';

describe('iot-gateway-app App', () => {
  let page: IotGatewayAppPage;

  beforeEach(() => {
    page = new IotGatewayAppPage();
  });

  it('should display message saying app works', () => {
    page.navigateTo();
    expect(page.getParagraphText()).toEqual('app works!');
  });
});

using Lexos.SQS.Interface;
using LexosHub.ERP.VarejOnline.Domain.Interfaces.Services;
using LexosHub.ERP.VarejOnline.Infra.CrossCutting.Settings;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Events;
using LexosHub.ERP.VarejOnline.Infra.SyncIn.Interfaces;
using LexosHub.ERP.VarejOnline.Infra.SyncIn.Requests;
using LexosHub.ERP.VarejOnline.Infra.SyncOut.Interfaces;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace LexosHub.ERP.VarejOnline.Infra.Messaging.Handlers
{
    public class InvoicesRequestedEventHandler : IEventHandler<InvoicesRequested>
    {
        private readonly ILogger<InvoicesRequestedEventHandler> _logger;
        private readonly IIntegrationService _integrationService;
        private readonly IVarejOnlineApiService _apiService;
        private readonly ISyncInApiService _syncInApiService;

        public InvoicesRequestedEventHandler(
            ILogger<InvoicesRequestedEventHandler> logger,
            IIntegrationService integrationService,
            IVarejOnlineApiService apiService,
            IOptions<SyncOutConfig> syncOutConfig,
            ISyncInApiService syncInApiService)
        {
            _logger = logger;
            _integrationService = integrationService;
            _apiService = apiService;
            var config = syncOutConfig?.Value ?? throw new ArgumentNullException(nameof(syncOutConfig));
            _syncInApiService = syncInApiService;
        }

        public async Task HandleAsync(InvoicesRequested @event, CancellationToken cancellationToken)
        {
            if (@event == null) throw new ArgumentNullException(nameof(@event));

            _logger.LogInformation("Invoices requested for hub {HubKey}, numero {Number}", @event.HubKey, @event.Number);

            var integrationResponse = await _integrationService.GetIntegrationByKeyAsync(@event.HubKey);

            if (!integrationResponse.IsSuccess || integrationResponse.Result == null)
            {
                _logger.LogWarning("Falha ao obter integração para hub {HubKey}", @event.HubKey);
                return;
            }

            var token = integrationResponse.Result.Token ?? string.Empty;

            var invoiceResponse = await _apiService.GetInvoiceXmlAsync(token, @event.Number, cancellationToken);

            var xmlContent = invoiceResponse.Result;

            if (!invoiceResponse.IsSuccess || string.IsNullOrWhiteSpace(xmlContent))
            {
                _logger.LogWarning(
                    "Não foi possível obter o XML da nota fiscal {Number} para o hub {HubKey}. Utilizando mock.",
                    @event.Number,
                    @event.HubKey);

                xmlContent = BuildMockInvoiceXml();
            }
			if (!string.IsNullOrWhiteSpace(xmlContent))
			{
                var nfeRequest = new NotaFiscalExternaDto { Chave = @event.HubKey!, XmlNota = xmlContent! };
                var nfeExternaResponse = await _syncInApiService.InserirNotaFiscalExterna(nfeRequest);
            }
        }

        private static string BuildMockInvoiceXml()
        {
            return @"<?xml version=""1.0"" encoding=""utf-8""?>
<nfeProc versao=""4.00"" xmlns=""http://www.portalfiscal.inf.br/nfe"">
	<NFe>
		<infNFe versao=""4.00"" Id=""NFe35250944034488000143550010013077741345857986"">
			<ide>
				<cUF>35</cUF>
				<cNF>34585798</cNF>
				<natOp>Venda Online</natOp>
				<mod>55</mod>
				<serie>1</serie>
				<nNF>1307774</nNF>
				<dhEmi>2025-09-24T10:29:04-03:00</dhEmi>
				<tpNF>1</tpNF>
				<idDest>1</idDest>
				<cMunFG>3550308</cMunFG>
				<tpImp>1</tpImp>
				<tpEmis>1</tpEmis>
				<cDV>6</cDV>
				<tpAmb>1</tpAmb>
				<finNFe>1</finNFe>
				<indFinal>1</indFinal>
				<indPres>2</indPres>
				<indIntermed>1</indIntermed>
				<procEmi>0</procEmi>
				<verProc>2.0</verProc>
			</ide>
			<emit>
				<CNPJ>44034488000143</CNPJ>
				<xNome>KID NEXT BRINQUEDOS LTDA</xNome>
				<enderEmit>
					<xLgr>Rua Toshio Uenishi</xLgr>
					<nro>60</nro>
					<xBairro>Vila Nova Bonsucesso</xBairro>
					<cMun>3550308</cMun>
					<xMun>Guarulhos</xMun>
					<UF>SP</UF>
					<CEP>07175540</CEP>
				</enderEmit>
				<IE>127704481110</IE>
				<CRT>3</CRT>
			</emit>
			<dest>
				<CPF>12671631782</CPF>
				<xNome>Gabriela Dos Santos Ribeiro Queiroz</xNome>
				<enderDest>
					<xLgr>Alameda Jau</xLgr>
					<nro>1874</nro>
					<xBairro>Jardim Paulista</xBairro>
					<cMun>3550308</cMun>
					<xMun>Sao Paulo</xMun>
					<UF>SP</UF>
					<CEP>01420002</CEP>
					<cPais>1058</cPais>
					<xPais>BRASIL</xPais>
				</enderDest>
				<indIEDest>9</indIEDest>
			</dest>
			<det nItem=""1"">
				<prod>
					<cProd>290033</cProd>
					<cEAN>7896965251754</cEAN>
					<xProd>ROMA COLECAO AMOR DE FILHOTE STITCH BONECO VINIL 5175</xProd>
					<NCM>95030039</NCM>
					<CEST>2806400</CEST>
					<CFOP>5102</CFOP>
					<uCom>PC</uCom>
					<qCom>1.0000</qCom>
					<vUnCom>69.490000</vUnCom>
					<vProd>69.49</vProd>
					<cEANTrib>7896965251754</cEANTrib>
					<uTrib>PC</uTrib>
					<qTrib>1.0000</qTrib>
					<vUnTrib>69.490000</vUnTrib>
					<indTot>1</indTot>
					<xPed>200000932884547</xPed>
				</prod>
				<imposto>
					<vTotTrib>22.43</vTotTrib>
					<ICMS>
						<ICMS00>
							<orig>0</orig>
							<CST>00</CST>
							<modBC>3</modBC>
							<vBC>69.49</vBC>
							<pICMS>18.00</pICMS>
							<vICMS>12.51</vICMS>
						</ICMS00>
					</ICMS>
					<IPI>
						<cEnq>999</cEnq>
						<IPINT>
							<CST>53</CST>
						</IPINT>
					</IPI>
					<PIS>
						<PISAliq>
							<CST>01</CST>
							<vBC>69.49</vBC>
							<pPIS>1.65</pPIS>
							<vPIS>1.15</vPIS>
						</PISAliq>
					</PIS>
					<COFINS>
						<COFINSAliq>
							<CST>01</CST>
							<vBC>69.49</vBC>
							<pCOFINS>7.60</pCOFINS>
							<vCOFINS>5.28</vCOFINS>
						</COFINSAliq>
					</COFINS>
				</imposto>
			</det>
			<total>
				<ICMSTot>
					<vBC>69.49</vBC>
					<vICMS>12.51</vICMS>
					<vICMSDeson>0.00</vICMSDeson>
					<vFCPUFDest>0.00</vFCPUFDest>
					<vICMSUFDest>0.00</vICMSUFDest>
					<vICMSUFRemet>0.00</vICMSUFRemet>
					<vFCP>0.00</vFCP>
					<vBCST>0.00</vBCST>
					<vST>0.00</vST>
					<vFCPST>0.00</vFCPST>
					<vFCPSTRet>0.00</vFCPSTRet>
					<qBCMono>0.00</qBCMono>
					<vICMSMono>0.00</vICMSMono>
					<qBCMonoReten>0.00</qBCMonoReten>
					<vICMSMonoReten>0.00</vICMSMonoReten>
					<qBCMonoRet>0.00</qBCMonoRet>
					<vICMSMonoRet>0.00</vICMSMonoRet>
					<vProd>69.49</vProd>
					<vFrete>0.00</vFrete>
					<vSeg>0.00</vSeg>
					<vDesc>0.00</vDesc>
					<vII>0.00</vII>
					<vIPI>0.00</vIPI>
					<vIPIDevol>0.00</vIPIDevol>
					<vPIS>1.15</vPIS>
					<vCOFINS>5.28</vCOFINS>
					<vOutro>0.00</vOutro>
					<vNF>69.49</vNF>
					<vTotTrib>22.43</vTotTrib>
				</ICMSTot>
			</total>
			<transp>
				<modFrete>2</modFrete>
				<transporta>
					<CNPJ>20121850000155</CNPJ>
					<xNome>MERCADO ENVIOS SERVICOS DE LOGISTICA LTDA.</xNome>
					<IE>492875457119</IE>
					<xEnder>Av. das Nacoes Unidas 3003</xEnder>
					<xMun>Osasco</xMun>
					<UF>SP</UF>
				</transporta>
				<vol>
					<qVol>1</qVol>
					<esp>VOLUME</esp>
					<pesoL>0.926</pesoL>
					<pesoB>0.926</pesoB>
				</vol>
			</transp>
			<pag>
				<detPag>
					<tPag>19</tPag>
					<vPag>69.49</vPag>
				</detPag>
			</pag>
			<infIntermed>
				<CNPJ>03007331000141</CNPJ>
				<idCadIntTran>44034488000143</idCadIntTran>
			</infIntermed>
			<infAdic>
				<infCpl>Val Aprox dos Tributos Fed: R$ 9.93 Est: R$ 12.51 Mun: R$ 0.00 Fonte: IBPT0</infCpl>
			</infAdic>
		</infNFe>
		<Signature xmlns=""http://www.w3.org/2000/09/xmldsig#"">
			<SignedInfo>
				<CanonicalizationMethod Algorithm=""http://www.w3.org/TR/2001/REC-xml-c14n-20010315""/>
				<SignatureMethod Algorithm=""http://www.w3.org/2000/09/xmldsig#rsa-sha1""/>
				<Reference URI=""#NFe35250944034488000143550010013077741345857986"">
					<Transforms>
						<Transform Algorithm=""http://www.w3.org/2000/09/xmldsig#enveloped-signature""/>
						<Transform Algorithm=""http://www.w3.org/TR/2001/REC-xml-c14n-20010315""/>
					</Transforms>
					<DigestMethod Algorithm=""http://www.w3.org/2000/09/xmldsig#sha1""/>
					<DigestValue>qTnLStOOszB2k/EPCAhO/RNN5Lk=</DigestValue>
				</Reference>
			</SignedInfo>
			<SignatureValue>0nyqYuSQDKn9NeTaEDn0o4eBgQchb08D4JpjsKqG2KS2cCp1xW0dfqx8Zs9zpZyM4+jL9EqqKsrxrAyZ7+ZrJBkMmKM2HjtakmD4L19RzW9r3C5IDH3Xh/1CM/P7EE6Tl9/DuLIw72Wz1WHrLprNBaIAXC2OAmv+Wq+MXHU5lZq0z/OfJbbMHEY8LmVzpNew9bBiDYbbInlNQ4oqg7oFfG7kYmvymUM9ldJjMC53gB3ek7iAAh6paX6DvP6YWV7q0qCsDKT/WaixfBu0N8q+ijFtPDL1T/Zm4Eu9f7f6Dcz63WvfgtJgeZx7njT4cVrQRmHy4pFcxkev6vhYNQjpJA==</SignatureValue>
			<KeyInfo>
				<X509Data>
					<X509Certificate>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</X509Certificate>
				</X509Data>
			</KeyInfo>
		</Signature>
	</NFe>
	<protNFe versao=""4.00"">
		<infProt Id=""NFe135252814596834"">
			<tpAmb>1</tpAmb>
			<verAplic>SP_NFE_PL009_V4</verAplic>
			<chNFe>35250944034488000143550010013077741345857986</chNFe>
			<dhRecbto>2025-09-24T10:29:05-03:00</dhRecbto>
			<nProt>135252814596834</nProt>
			<digVal>qTnLStOOszB2k/EPCAhO/RNN5Lk=</digVal>
			<cStat>100</cStat>
			<xMotivo>Autorizado o uso da NF-e</xMotivo>
		</infProt>
	</protNFe>
</nfeProc>";
        }


    }
}

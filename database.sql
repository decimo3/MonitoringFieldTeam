-- Schema for MonitoringFieldTeam SQLite database
-- Created to match INSERT usage in src/Helpers/Database.cs

PRAGMA foreign_keys = ON;

CREATE TABLE IF NOT EXISTS ordenacao (
  identifier BIGINT PRIMARY KEY AUTOINCREMENT,
  order_number BIGINT NOT NULL,
  status_code INTEGER NOT NULL,
  created_at TIMESTAMP NOT NULL,
  updated_at TIMESTAMP NOT NULL,
  observation TEXT DEFAULT NULL,
);

-- General information about activities
CREATE TABLE IF NOT EXISTS general (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  data TEXT,
  notaservico TEXT,
  recurso TEXT,
  atividade TEXT,
  situacao TEXT,
  damage TEXT,
  vencimento TEXT,
  descricao TEXT,
  observacao TEXT
);
CREATE INDEX IF NOT EXISTS idx_general_notaservico ON general(notaservico);

-- Finalization / parts used in a service
CREATE TABLE IF NOT EXISTS finaliza (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  notaservico TEXT,
  codigo TEXT,
  quantidade TEXT
);
CREATE INDEX IF NOT EXISTS idx_finaliza_notaservico ON finaliza(notaservico);

-- Materials used / delivered
CREATE TABLE IF NOT EXISTS material (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  nota TEXT,
  tipo TEXT,
  codigo TEXT,
  serie TEXT,
  descricao TEXT,
  quantidade TEXT,
  origem TEXT
);
CREATE INDEX IF NOT EXISTS idx_material_nota ON material(nota);

-- Ocorrencia / inspection reports
CREATE TABLE IF NOT EXISTS ocorrencia (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  notaservico TEXT,
  caixatipo TEXT,
  caixamodelo TEXT,
  numerotoi TEXT,
  nometitular TEXT,
  documentotipo TEXT,
  documentonum TEXT,
  residenciaclasse TEXT,

  motivoinspecao TEXT,
  instalacaosuspensa TEXT,
  instalacaonormalizada TEXT,
  consumidoracompanhou TEXT,
  clienteautorizoulevantamento TEXT,
  clientesolicitoupericia TEXT,
  clientequalassinou TEXT,
  clienterecusouassinar TEXT,
  clienterecusoureceber TEXT,
  fisicoentreguetoi TEXT,
  quantidadeevidencias TEXT,
  existenciaevidencias TEXT,
  descricaoirregularidade TEXT,

  grupotarifarico TEXT,
  ligacaotipo TEXT,
  quantidadeelementos TEXT,
  fornecimentotipo TEXT,
  tensaotipo TEXT,
  tensaonivel TEXT,
  ramaltipo TEXT,
  sistemaencapsulado TEXT,

  medidortipo TEXT,
  medidornumero TEXT,
  medidormarca TEXT,
  medidorano TEXT,
  medidorpatrimonio TEXT,
  medidortensao TEXT,
  medidoranominal TEXT,
  medidoramaximo TEXT,
  medidorconstante TEXT,
  medidorlocalizacao TEXT,
  medidorobservacao TEXT,

  declarantenomecompleto TEXT,
  declarantegrauafiinidade TEXT,
  declarantedocumento TEXT,
  declarantetempoocupacao TEXT,
  declarantetempounidade TEXT,
  declarantetipoocupacao TEXT,
  declaranteqntresidentes TEXT,
  declaranteemail TEXT,
  declarantecelular TEXT,

  selagemtampos TEXT,
  selagembornes TEXT,
  selagemparafuso TEXT,
  selagemtrava TEXT,
  selagemtampa TEXT,
  selagembase TEXT,
  selagemgeral TEXT
);
CREATE INDEX IF NOT EXISTS idx_ocorrencia_notaservico ON ocorrencia(notaservico);

-- End of schema
